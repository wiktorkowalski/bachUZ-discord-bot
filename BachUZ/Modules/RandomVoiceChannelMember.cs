using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BachUZ.Modules
{
    public class RandomVoiceChannelMember : ModuleBase<SocketCommandContext>
    {
        [Command("pickrandom")]
        [Alias("pr", "doodpowiedzi", "dotablicy")]
        [Summary("Picks random user from voice channel")]
        [RequireContext(ContextType.Guild)]
        public async Task PickRandomVoiceMamber()
        {
            var random = new Random();
            var user = Context.User as SocketGuildUser;
            var voiceChannel = user?.VoiceChannel;
            if (voiceChannel == null)
            {
                await ReplyAsync("Nie jesteś na żadnym kanale głosowym.");
                return;
            }
            var r = random.Next(voiceChannel.Users.Count);
            var randomMember = voiceChannel.Users.ElementAt(r);
            await ReplyAsync($"Raz, dwa, trzy, do odpowiedzi idziesz Ty <@{randomMember.Id}>");
        }
    }
}
