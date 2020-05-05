using System;
using System.Linq;
using System.Threading.Tasks;
using BachUZ.Database;
using BachUZ.Services;
using Discord.WebSocket;

namespace BachUZ.Events
{
    static class UserVoiceStateUpdated
    {

        private static async Task HandleJoin(SocketUser user, SocketVoiceState after)
        {
            await Task.Run(() =>
            {
                var usr = new UserOnVoice
                {
                    UserId = user.Id,
                    GuildId = after.VoiceChannel.Guild.Id,
                    VoiceChannelId = after.VoiceChannel.Id,
                    JoinTime = DateTime.Now
                };

                VoicechatMonitoringService.UsersOnVoiceChannels.TryAdd(user.Id, usr);
            });
        }

        private static async Task HandleLeave(SocketUser user)
        {
            VoicechatMonitoringService.UsersOnVoiceChannels.TryRemove(user.Id, out var leavingUser);
            if (leavingUser != null)
            {
                var timeOnVoice = Convert.ToInt32((DateTime.Now - leavingUser.JoinTime).TotalSeconds);
                await using (var database = new BachuzContext())
                {
                    var voiceUser = database.UsersOnVoice
                        .AsQueryable()
                        .Select(item => item)
                        .FirstOrDefault(item =>
                            item.GuildId == leavingUser.GuildId &&
                            item.UserId == leavingUser.UserId &&
                            item.VoiceChannelId == leavingUser.VoiceChannelId
                        );

                    if (voiceUser == null)
                    {
                        var userOnVoice = new UsersOnVoice
                        {
                            UserId = leavingUser.UserId,
                            VoiceChannelId = leavingUser.VoiceChannelId,
                            GuildId = leavingUser.GuildId,
                            Time = timeOnVoice
                        };
                        await database.UsersOnVoice.AddAsync(userOnVoice);
                        await database.SaveChangesAsync();
                    }
                    else
                    {
                        voiceUser.Time += timeOnVoice;
                        await database.SaveChangesAsync();
                    }
                }
            }
        }

        internal static async Task HandleEvent(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            await using (var database = new BachuzContext())
            {
                var inUserTable = database.Users.AsQueryable().FirstOrDefault(item => item.UserId == user.Id);
                if (inUserTable == null)
                {
                    var dbUser = new Users
                    {
                        UserId = user.Id,
                        Points = 0,
                        LastUsedDaily = null
                    };
                    await database.Users.AddAsync(dbUser);
                    await database.SaveChangesAsync();
                }
            }

            if (before.VoiceChannel == null && after.VoiceChannel != null)
            {
                await HandleJoin(user, after);
            }
            else if (before.VoiceChannel != null && after.VoiceChannel == null)
            {
                await HandleLeave(user);

            }
            else if (before.VoiceChannel != null && (after.VoiceChannel != null && before.VoiceChannel.Id != after.VoiceChannel.Id))
            {
                await HandleLeave(user);
                await HandleJoin(user, after);
            }
        }
    }
}
