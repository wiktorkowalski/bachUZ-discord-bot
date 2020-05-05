using System;
using System.Collections.Generic;

namespace BachUZ.Database
{
    public partial class UsersGuilds
    {
        public decimal UserId { get; set; }
        public decimal GuildId { get; set; }

        public virtual Guilds Guild { get; set; }
        public virtual Users User { get; set; }
    }
}
