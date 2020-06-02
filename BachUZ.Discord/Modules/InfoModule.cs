using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BachUZ.Database;
using BachUZ.Discord.Utils;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BachUZ.Modules
{
    public enum ResultType
    {
        General, Detailed
    }
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private const string EmptyString = "\u200B"; // discord embeds don't accept empty string, so we use 0 width space.

        [Command("ping")]
        [Summary("Checks if bot is alive or not.")]
        public async Task Ping()
        {
            var sw = Stopwatch.StartNew();
            var msg = await Context.Channel.SendMessageAsync("Pong!").ConfigureAwait(false);
            sw.Stop();
            await msg.ModifyAsync(m => m.Content = $"Pong! ({(int)sw.Elapsed.TotalMilliseconds}ms)").ConfigureAwait(false);
        }

        [Command("userinfo")]
        [Alias("ui", "whois")]
        [Summary("Returns data about a guildUser")]
        [RequireContext(ContextType.Guild)]
        public async Task UserInfo(IGuildUser? usr = null)
        {
            var user = usr ?? Context.User as IGuildUser;
            if (user == null)
            {
                return;
            }

            var roleNames = user.RoleIds
                .Select(roleId => Context.Guild.GetRole(roleId))
                .Where(role => !role.IsEveryone).Select(role => role.Name).ToList();

            var permissions = user.GuildPermissions.ToList();

            var embed = new EmbedBuilder()
                .AddField("Name", $"**{user.Username}**", true)
                .AddField("Id", user.Id, true)
                .AddField(EmptyString, EmptyString, true)
                .AddField("Joined Server at", $"{user.JoinedAt:dd.MM.yyyy HH:mm}", true)
                .AddField("Joined Discord at", $"{user.CreatedAt:dd.MM.yyyy HH:mm}", true)
                .AddField(EmptyString, EmptyString, true)
                .AddField("User Status", user.Status, true)
                .AddField("User Roles", string.Join('\n', roleNames), true)
                .AddField("User Permissions", string.Join('\n', permissions), true)
                .WithThumbnailUrl(user.GetAvatarUrl())
                .Build();

            await Context.Channel.SendMessageAsync(string.Empty, false, embed).ConfigureAwait(false);
        }

        [Command("serverinfo")]
        [Alias("si", "guildinfo")]
        [Summary("Returns data about current guild")]
        [RequireContext(ContextType.Guild)]
        public async Task GuildInfo()
        {
            var features = string.Join("\n", Context.Guild.Features);
            if (string.IsNullOrEmpty(features))
            {
                features = "-";
            }
            var embed = new EmbedBuilder()
                .WithAuthor("Server info")
                .WithTitle(Context.Guild.Name)
                .AddField("Id", Context.Guild.Id, true)
                .AddField("Owner", Context.Guild.Owner.Username, true)
                .AddField("Members", Context.Guild.MemberCount, true)
                .AddField("Text channels", Context.Guild.TextChannels.Count, true)
                .AddField("Voice channels", Context.Guild.VoiceChannels.Count, true)
                .AddField("Created at", $"{Context.Guild.CreatedAt:dd.MM.yyyy HH:mm}", true)
                .AddField("Voice region", Context.Guild.VoiceRegionId, true)
                .AddField("Roles", Context.Guild.Roles.Count, true)
                .AddField("Features", features, true)
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .Build();

            await Context.Channel.SendMessageAsync(string.Empty, false, embed).ConfigureAwait(false);
        }

        [Command("emotesinfo")]
        [Alias("ei")]
        [Summary("Returns data about emotes usage in this guild")]
        [RequireContext(ContextType.Guild)]
        public async Task EmotesInfo()
        {
            await using (var database = new BachuzContext())
            {
                var emotes = await database.Emotes.AsQueryable().Where(emote => emote.Guild.GuildId == Context.Guild.Id).OrderByDescending(emote => emote.Count).ToListAsync();
                var sb = new StringBuilder();
                sb.AppendLine("Emotes usage in this server:");
                foreach (var emote in emotes)
                {
                    sb.AppendLine($"<:{emote.Name}:{emote.EmoteId}> - {emote.Count}");
                }
                var chunks = Utilities.SplitMessage(sb.ToString());
                foreach (var chunk in chunks)
                {
                    await Context.Channel.SendMessageAsync(chunk,
                            allowedMentions: AllowedMentions.None)
                        .ConfigureAwait(false);
                }
            }
        }

        [Command("voiceInfo")]
        [Alias("vi")]
        [Summary("Return talk usage")]
        [RequireContext(ContextType.Guild)]
        public async Task VoiceInfo(ResultType resultType = ResultType.General)
        {
            await using (var database = new BachuzContext())
            {
                if (resultType == ResultType.Detailed)
                {
                    var usersOnVoice = await database.UsersOnVoice.AsQueryable().Where(user => user.Guild.GuildId == Context.Guild.Id).OrderByDescending(user => user.Time).ToListAsync();
                    var sb = new StringBuilder();
                    sb.AppendLine("Time spend on voice channels in this server:");
                    foreach (var user in usersOnVoice)
                    {
                        sb.AppendLine($"<@{user.UserId}> <#{user.VoiceChannelId}> - {TimeSpan.FromSeconds(user.Time)}");
                    }

                    var chunks = Utilities.SplitMessage(sb.ToString());
                    foreach (var chunk in chunks)
                    {
                        await Context.Channel.SendMessageAsync(chunk,
                                allowedMentions: AllowedMentions.None)
                            .ConfigureAwait(false);
                    }
                }
                else if (resultType == ResultType.General)
                {
                    var usersOnVoice = await database.UsersOnVoice.AsQueryable()
                        .Where(user => user.Guild.GuildId == Context.Guild.Id)
                        .GroupBy(user => user.UserId)
                        .Select(o => new { UserId = o.Key, Time = o.Sum(s => s.Time) })
                        .OrderByDescending(user => user.Time)
                        .ToListAsync();
                    var sb = new StringBuilder();
                    sb.AppendLine("Time spend on voice channels in this server:");
                    foreach (var user in usersOnVoice)
                    {
                        sb.AppendLine($"<@{user.UserId}> - {TimeSpan.FromSeconds(user.Time)}");
                    }
                    var chunks = Utilities.SplitMessage(sb.ToString());
                    foreach (var chunk in chunks)
                    {
                        await Context.Channel.SendMessageAsync(chunk,
                                allowedMentions: AllowedMentions.None)
                            .ConfigureAwait(false);
                    }
                }

            }
        }

        [Command("voiceInfo")]
        [Alias("vi")]
        [Summary("Return talk usage")]
        [RequireContext(ContextType.Guild)]
        public async Task VoiceInfo(SocketUser guildUser, ResultType resultType = ResultType.General)
        {
            await using (var database = new BachuzContext())
            {
                if (resultType == ResultType.General)
                {
                    var userData = database.UsersOnVoice.AsQueryable()
                        .Where(user => user.Guild.GuildId == Context.Guild.Id && user.UserId == guildUser.Id)
                        .GroupBy(user => user.UserId)
                        .Select(o => new { UserId = o.Key, Time = o.Sum(s => s.Time) })
                        .OrderByDescending(user => user.Time)
                        .FirstOrDefault();


                    await Context.Channel.SendMessageAsync($"<@{userData.UserId}> total time on voice channels in this guild - {TimeSpan.FromSeconds(userData.Time)}",
                            allowedMentions: AllowedMentions.None)
                        .ConfigureAwait(false);
                }
                else
                {
                    var usersOnVoice = await database.UsersOnVoice.AsQueryable()
                        .Where(user => user.Guild.GuildId == Context.Guild.Id && user.UserId == guildUser.Id)
                        .OrderByDescending(user => user.Time)
                        .ToListAsync();

                    var response = new StringBuilder();
                    foreach (var user in usersOnVoice)
                    {
                        response.AppendLine(
                            $"<@{user.UserId}> <#{user.VoiceChannelId}> - {TimeSpan.FromSeconds(user.Time)}");
                    }

                    var chunks = Utilities.SplitMessage(response.ToString());
                    foreach (var chunk in chunks)
                    {
                        await Context.Channel.SendMessageAsync(chunk,
                                allowedMentions: AllowedMentions.None)
                            .ConfigureAwait(false);
                    }
                }
            }
        }
    }
}