using System.Diagnostics;
using System.Threading.Tasks;
using Discord.Commands;

namespace BachUZ.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Checks if bot is alive or not.")]
        public async Task Ping()
        {
            var sw = Stopwatch.StartNew();
            var msg = await Context.Channel.SendMessageAsync("Pong!").ConfigureAwait(false);
            sw.Stop();
            await msg.ModifyAsync(m => m.Content = $"Pong! ({(int)sw.Elapsed.TotalMilliseconds}ms)").ConfigureAwait(false);
        }
    }
}