using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BachUZ.Modules
{
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
        [Summary("Returns data about a user")]
        [RequireContext(ContextType.Guild)]
        public async Task UserInfo(IGuildUser usr = null)
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
    }
}