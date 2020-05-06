using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BachUZ.Database;
using Discord.WebSocket;

namespace BachUZ.Events
{
    static class JoinedGuild
    {
        internal static async Task HandleEvent(SocketGuild guild)
        {
            await using (var database = new BachuzContext())
            {
                Guilds newGuild = new Guilds
                {
                    GuildId = guild.Id
                };
                await database.Guilds.AddAsync(newGuild);

                var emotes = new List<Emotes>();

                foreach (var guildEmote in guild.Emotes)
                {
                    emotes.Add(new Emotes
                    {
                        EmoteId = guildEmote.Id,
                        Name = guildEmote.Name,
                        Count = 0,
                        GuildId = guild.Id,
                    });
                }

                await database.Emotes.AddRangeAsync(emotes);
                await database.SaveChangesAsync();
            }
        }
    }
}
