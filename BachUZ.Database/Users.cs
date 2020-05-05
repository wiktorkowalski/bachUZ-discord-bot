using System;
using System.Collections.Generic;

namespace BachUZ.Database
{
    public partial class Users
    {
        public Users()
        {
            UsersGuilds = new HashSet<UsersGuilds>();
            UsersOnVoice = new HashSet<UsersOnVoice>();
        }

        public decimal UserId { get; set; }
        public int Points { get; set; }
        public DateTime? LastUsedDaily { get; set; }

        public virtual ICollection<UsersGuilds> UsersGuilds { get; set; }
        public virtual ICollection<UsersOnVoice> UsersOnVoice { get; set; }
    }
}
