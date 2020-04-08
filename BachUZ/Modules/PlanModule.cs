using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using AngleSharp;
using AngleSharp.Html.Dom;
using System;

namespace BachUZ.Modules
{
    public class PlanModule : ModuleBase<SocketCommandContext>
    {
        [Command("plan", RunMode = RunMode.Async)]
        [Summary("Looks for a specified group class schedule")]
        public async Task Plan(string objectID)
        {
            string response = "Trwa szukanie twojego planu...";
            var msg = await Context.Channel.SendMessageAsync(response).ConfigureAwait(false);
            var sw = Stopwatch.StartNew();
            bool compareID = int.TryParse(objectID, out int _);
            if (GroupScheduleCache.cacheTime == null || DateTime.Now.Subtract(GroupScheduleCache.cacheTime) > GroupScheduleCache.cacheTimeSpan)
            {
                GroupScheduleCache.cacheTime = DateTime.Now;
                await msg.ModifyAsync(m => m.Content = response + "\nRenewing cache...").ConfigureAwait(false);
                using (var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader()))
                {
                    var document = await context.OpenAsync("http://www.plan.uz.zgora.pl/grupy_lista_kierunkow.php");
                    var hrefs = document.QuerySelectorAll("a").OfType<IHtmlAnchorElement>();
                    GroupScheduleCache.hrefGroupList = new List<string>();
                    GroupScheduleCache.hrefObjectList = new List<string>();
                    foreach (var item in hrefs)
                    {
                        if (item.Href.Contains("pId_kierunek")) GroupScheduleCache.hrefGroupList.Add(item.Href);
                    }
                    foreach (var item in GroupScheduleCache.hrefGroupList)
                    {
                        var groupDocument = await context.OpenAsync(item);
                        var groupHrefs = groupDocument.QuerySelectorAll("a").OfType<IHtmlAnchorElement>();
                        foreach (var innerItem in groupHrefs)
                        {
                            if (innerItem.Href.Contains("pId_Obiekt")) GroupScheduleCache.hrefObjectList.Add(innerItem.Href);
                        }
                    }
                }
            }
            else
            {
                response += "\nUsed data from cache";
                await msg.ModifyAsync(m => m.Content = response).ConfigureAwait(false);
            }
            //search
            if (compareID)
            {
                foreach (var item in GroupScheduleCache.hrefObjectList)
                {
                    if (item.Contains(objectID)) response += "\nPlan twojej grupy:\n" + item;
                }
            }
            else
            {
                //foreach (var item in hrefObjectList)
                //{

                //    if (item.Contains(objectID)) response = "Plan twojej grupy:\n" + item;
                //}
            }

            sw.Stop();
            await msg.ModifyAsync(m => m.Content += response + $"\nCzas oczekiwania {sw.ElapsedMilliseconds}ms").ConfigureAwait(false);
        }
    }
}
