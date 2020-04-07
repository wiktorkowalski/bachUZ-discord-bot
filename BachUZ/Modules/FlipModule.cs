using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace BachUZ.Modules
{
    public class FlipModule : ModuleBase<SocketCommandContext>
    {
        [Command("toss")]
        [Summary("Tosses a coin")]
        public async Task Toss()
        {
            Random rand = new Random();
            var tossResult = rand.Next(2) == 1 ? "tails" : "heads";
            var msg = await Context.Channel.SendMessageAsync($"Coin shows {tossResult}").ConfigureAwait(false);
        }
    }
}
