using System.Threading.Tasks;
using Discord.Commands;

namespace BachUZ.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Checks if bot is alive or not.")]
        public Task Ping() => ReplyAsync("pong!");
    }
}