using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BachUZ.Website.Data
{
    public static class LoginState
    {
        public static bool IsLoggedIn { get; set; } = false;

        public static UserInfo UserInfo { get; set; }

        public static TokenResponse TokenResponse { get; set; }
    }
}
