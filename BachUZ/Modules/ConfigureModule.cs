using BachUZ.Database;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace BachUZ.Modules
{
    public class ConfigureModule : ModuleBase<SocketCommandContext>
    {

        [Command("setprefix")]
        [Alias("sp")]
        [Summary("Sets command prefix for this guild")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Permission")]
        [RequireUserPermission(GuildPermission.ManageGuild, Group = "Permission")]
        public async Task SetPrefix(string prefix)
        {
            if (prefix.Length > 6)
            {
                await ReplyAsync("Prefix cannot be longer than 6 characters.");
                return;
            }

            if (string.IsNullOrEmpty(prefix))
            {
                await ReplyAsync("Prefix cannot be empty");
                return;
            }

            Program.CustomPrefixes.AddOrUpdate(Context.Guild.Id, prefix, (key, oldValue) => prefix);
            await using (var database = new BachuzContext())
            {
                var guild = database.Guilds.AsQueryable().FirstOrDefault(item => item.GuildId == Context.Guild.Id);
                guild.CustomPrefix = prefix;
                await database.SaveChangesAsync();
            }

            await Context.Channel.SendMessageAsync($"Prefix has been set to: {prefix}").ConfigureAwait(false);
        }
    }
}
