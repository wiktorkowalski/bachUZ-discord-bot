using System;
using System.Collections.Generic;

namespace BachUZ.Database
{
    public partial class UsersOnVoice
    {
        public decimal UserId { get; set; }
        public decimal VoiceChannelId { get; set; }
        public decimal GuildId { get; set; }
        public int Time { get; set; }

        public virtual Guilds Guild { get; set; }
        public virtual Users User { get; set; }
    }
}
