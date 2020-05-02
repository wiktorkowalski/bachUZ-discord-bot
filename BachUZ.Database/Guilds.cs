using System;
using System.Collections.Generic;

namespace BachUZ.Database
{
    public partial class Guilds
    {
        public Guilds()
        {
            Emotes = new HashSet<Emotes>();
            UsersGuilds = new HashSet<UsersGuilds>();
            UsersOnVoice = new HashSet<UsersOnVoice>();
        }

        public decimal GuildId { get; set; }
        public string CustomPrefix { get; set; }

        public virtual ICollection<Emotes> Emotes { get; set; }
        public virtual ICollection<UsersGuilds> UsersGuilds { get; set; }
        public virtual ICollection<UsersOnVoice> UsersOnVoice { get; set; }
    }
}
