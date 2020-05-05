using BachUZ.Services;
using Discord.Commands;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BachUZ.Modules
{
    public class ScheduleModule : ModuleBase<SocketCommandContext>
    {
        [Command("schedule", RunMode = RunMode.Async)]
        [Alias("plan")]
        [Summary("Looks for a specified group class schedule")]
        public async Task Schedule(string groupName)
        {
            if (UZScheduleService.ScheduleCache.Count == 0)
            {
                var sw = Stopwatch.StartNew();
                var msg = await Context.Channel.SendMessageAsync("Renewing schedules cache...");
                await UZScheduleService.PopulateScheduleCache();
                sw.Stop();
                await msg.ModifyAsync(m => m.Content = $"Cache has been renewed! ({(int)sw.Elapsed.TotalMilliseconds}ms)").ConfigureAwait(false);
            }
            var matchingKeys = UZScheduleService.ScheduleCache.Keys.Where(x => x.Contains(groupName.ToLower()));
            if (matchingKeys.Count() > 1)
            {
                var matchingGroups = string.Join("\n", matchingKeys.ToArray());
                if (matchingGroups.Length > 1950)
                {
                    await Context.Channel.SendMessageAsync("Too many possibilities. Please be more specific.");
                    return;
                }
                await Context.Channel.SendMessageAsync($"Specify the group:\n{matchingGroups}");
            }
            else if (!matchingKeys.Any())
            {
                await Context.Channel.SendMessageAsync("Class schedule not found.");
            }
            else
            {
                var matchingKey = matchingKeys.ToArray()[0];
                await Context.Channel.SendMessageAsync(
                    $"Class schedule for {matchingKey}: http://plan.uz.zgora.pl/{UZScheduleService.ScheduleCache[matchingKey]}");
            }
        }
    }
}
