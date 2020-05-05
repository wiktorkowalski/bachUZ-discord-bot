using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BachUZ.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;
        public CommandHandlingService(IConfiguration config, IServiceProvider services, DiscordSocketClient client, CommandService commands)
        {
            _config = config;
            _services = services;
            _client = client;
            _commands = commands;
        }

        public async Task InstallCommandsAsync()
        {
            // Pass the service provider to the second parameter of
            // AddModulesAsync to inject dependencies to all modules 
            // that may require them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            var argPos = 0;

            string prefix = _config["prefix"];

            // Checks for guild custom prefixes
            if (message.Channel is IGuildChannel guildChannel)
            {
                if (Program.CustomPrefixes.TryGetValue(guildChannel.GuildId, out var customPrefix))
                {
                    prefix = customPrefix;
                }
            }



            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasStringPrefix(prefix, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.

            // Keep in mind that result does not indicate a return value
            // rather an object stating if the command executed successfully.
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (result.Error == CommandError.UnknownCommand)
            {
                return;
            }
            // Optionally, we may inform the user if the command fails
            // to be executed; however, this may not always be desired,
            // as it may clog up the request queue should a user spam a
            // command.
            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}