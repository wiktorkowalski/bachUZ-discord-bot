using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using AngleSharp;
using AngleSharp.Html.Dom;
using System;
using BachUZ.Services;

namespace BachUZ.Modules
{
    public class ScheduleModule : ModuleBase<SocketCommandContext>
    {
        [Command("schedule")]
        [Alias("plan")]
        [Summary("Looks for a specified group class schedule")]
        public async Task Schedule(string groupName)
        {
            var matchingKeys = UZScheduleService.ScheduleCache.Keys.Where(x => x.Contains(groupName.ToLower()));
            if (matchingKeys.Count() > 1)
            {
                var matchingGroups = string.Join(", ", matchingKeys.ToArray());
                //var url = PlanService.ScheduleCache[groupName];
                await Context.Channel.SendMessageAsync($"Specify the group: {matchingGroups}.");
            }
            else if (!matchingKeys.Any())
            {
                await Context.Channel.SendMessageAsync("Class schedule not found.");
            }
            else
            {
                var matchingKey = matchingKeys.ToArray()[0];
                await Context.Channel.SendMessageAsync(
                    $"Class schedule: http://plan.uz.zgora.pl/{UZScheduleService.ScheduleCache[matchingKey]}");
            }
        }
    }
}
