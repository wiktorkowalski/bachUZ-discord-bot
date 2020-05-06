using AngleSharp;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BachUZ.Services
{
    class UZScheduleService
    {
        public static Dictionary<string, string> ScheduleCache = new Dictionary<string, string>();

        public static async Task PopulateScheduleCache()
        {
            using var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            var listOfSubjectsDoc = await context.OpenAsync("http://www.plan.uz.zgora.pl/grupy_lista_kierunkow.php");
            var subjectsATag = listOfSubjectsDoc.QuerySelector("body > div.container.main > ul").QuerySelectorAll("a").OfType<IHtmlAnchorElement>();
            foreach (var subjectATag in subjectsATag)
            {
                var groupListDoc = await context.OpenAsync(subjectATag.Href);
                var groupsNode = groupListDoc.QuerySelector("body > div.container.main > table").QuerySelectorAll("td");
                foreach (var group in groupsNode)
                {
                    var groupName = group.TextContent.ToLower().Trim();
                    var scheduleUrl = group.QuerySelector("a").GetAttribute("href");
                    Console.WriteLine($"{groupName} {scheduleUrl}");
                    ScheduleCache.Add(groupName, scheduleUrl);
                }
            }
        }
    }
}
