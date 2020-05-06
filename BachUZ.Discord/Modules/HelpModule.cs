using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace BachUZ.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        private readonly IConfiguration _config;

        public HelpModule(CommandService commandService, IConfiguration config)
        {
            _commandService = commandService;
            _config = config;
        }

        [Command("help")]
        [Summary("Displays a help command")]
        public async Task Help()
        {
            var prefix = _config["prefix"];
            var embedBuilder = new EmbedBuilder();
            embedBuilder.Description = "List of available commands:";

            foreach (var module in _commandService.Modules)
            {
                var descriptionBuilder = new StringBuilder();

                foreach (var command in module.Commands)
                {
                    var result = await command.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                    {
                        descriptionBuilder.AppendLine($"{prefix}{command.Aliases.First()}");
                    }
                }

                var description = descriptionBuilder.ToString();
                if (!string.IsNullOrWhiteSpace(description))
                {
                    embedBuilder.AddField(module.Name, description);
                }
            }

            await ReplyAsync("", false, embedBuilder.Build());

        }

        [Command("help")]
        [Summary("Displays a help command")]
        public async Task Help(string command)
        {
            var result = _commandService.Search(Context, command);
            if (!result.IsSuccess)
            {
                await ReplyAsync("Command not found!");
                return;
            }

            var embedBuilder = new EmbedBuilder();
            embedBuilder.Description = "Command details";
            foreach (var commandMatch in result.Commands)
            {
                var cmd = commandMatch.Command;

                embedBuilder.AddField(
                    string.Join(", ", cmd.Aliases),
                    $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                    $"Summary: {cmd.Summary}"
                );
            }

            await ReplyAsync("", false, embedBuilder.Build());
        }
    }
}
