using System.Threading.Tasks;
using BachUZ.Database;
using Discord.WebSocket;
using System.Linq;

namespace BachUZ.Events
{
    class LeftGuild
    {
        internal static async Task HandleEvent(SocketGuild guild)
        {
            await using (var database = new BachuzContext())
            {
                database.Remove(database.Guilds.Single(g => g.GuildId == guild.Id));
                await database.SaveChangesAsync();
            }
        }
    }
}
