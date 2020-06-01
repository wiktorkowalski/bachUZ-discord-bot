using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BachUZ.Database;
using Discord;
using Discord.WebSocket;

namespace BachUZ.Events
{
    static class MessageReceived
    {
        private static readonly Regex EmoteRegex = new Regex(@"<:\w*:\d*>", RegexOptions.Compiled);
        internal static async Task HandleEvent(SocketMessage message)
        {

            if (message.Channel is ITextChannel)
            {
                var matches = EmoteRegex.Matches(message.Content);

                if (matches.Count > 0)
                {
                    await using (var database = new BachuzContext())
                    {
                        foreach (Match match in matches)
                        {
                            var firstColonIndex = match.Value.IndexOf(':');
                            var secondColonIndex = match.Value.IndexOf(':', firstColonIndex + 1);
                            var emoteId = match.Value.Substring(secondColonIndex + 1);
                            emoteId = emoteId.Substring(0, emoteId.Length - 1);
                            var emote = await database.Emotes.FirstAsync(e => e.EmoteId == ulong.Parse(emoteId));
                            emote.Count += 1;
                            await database.SaveChangesAsync();
                        }
                    }
                }
            }
        }

    }
}
