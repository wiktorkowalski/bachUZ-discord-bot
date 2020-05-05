using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

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
