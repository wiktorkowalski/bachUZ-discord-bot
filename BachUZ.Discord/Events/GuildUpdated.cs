using System;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using BachUZ.Database;
using Discord;

namespace BachUZ.Events
{
    static class GuildUpdated
    {
        internal static async Task HandleEvent(SocketGuild before, SocketGuild after)
        {
            if (before.Emotes.Count > after.Emotes.Count)
            {
                var removedEmote = before.Emotes.Except(after.Emotes).First();
                await using (var database = new BachuzContext())
                {
                    database.Remove(database.Emotes.Single(e => e.EmoteId == removedEmote.Id));
                    await database.SaveChangesAsync();
                }
            }
            else if (after.Emotes.Count > before.Emotes.Count)
            {
                var addedEmote = after.Emotes.Except(before.Emotes).First();

                await using (var database = new BachuzContext())
                {
                    Emotes newEmote = new Emotes
                    {
                        EmoteId = addedEmote.Id,
                        GuildId = after.Id,
                        Name = addedEmote.Name,
                        Count = 0
                    };

                    await database.Emotes.AddAsync(newEmote);
                    await database.SaveChangesAsync();
                }

            }
            else
            {
                // updates Emote name in database
                var updatedEmote =
                    (from emoteAfter in after.Emotes
                     let emoteBefore = before.Emotes
                         .First(e => e.Id == emoteAfter.Id)
                     where emoteAfter.Name != emoteBefore.Name
                     select emoteAfter)
                    .FirstOrDefault();

                if (updatedEmote != null)
                {
                    await using (var database = new BachuzContext())
                    {
                        var emote = database.Emotes.First(e => e.EmoteId == updatedEmote.Id);
                        emote.Name = updatedEmote.Name;
                        await database.SaveChangesAsync();
                    }
                }
            }
        }
    }
}