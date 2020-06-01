using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BachUZ.Website.Data
{
    public class UserInfo
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Avatar { get; set; }
        public bool Verified { get; set; }
        public string Email { get; set; }
        public int Flags { get; set; }
        public int Premium_type { get; set; }
        public int Public_flags { get; set; }

    }
}
