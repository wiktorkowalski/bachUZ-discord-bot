using System;
using System.Collections.Generic;

namespace BachUZ.Database
{
    public partial class Emotes
    {
        public decimal EmoteId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal? GuildId { get; set; }

        public virtual Guilds Guild { get; set; }
    }
}
