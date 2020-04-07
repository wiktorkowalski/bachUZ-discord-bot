using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace BachUZ.Modules
{
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        [Command("roll")]
        [Summary("Rolls a 6 sided dice")]
        public async Task Roll()
        {
            Random rand = new Random();
            var rollResult = rand.Next(1, 7);
            var msg = await Context.Channel.SendMessageAsync($"Dice shows {rollResult}").ConfigureAwait(false);
        }
    }
}
