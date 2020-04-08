using System;
using System.Collections.Generic;

namespace BachUZ.Modules
{
    public static class GroupScheduleCache
    {
        public static List<string> hrefGroupList = new List<string>();
        public static List<string> hrefObjectList = new List<string>();
        public static DateTime cacheTime;
        public static TimeSpan cacheTimeSpan = new TimeSpan(0, 30, 0);

    }
}
