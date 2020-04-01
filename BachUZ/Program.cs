using System;
using System.IO;
using System.Threading.Tasks;
using BachUZ.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace BachUZ
{
    class Program
    {
        private IConfiguration _config;
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _config = BuildConfig();

            var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, _config["token"]);
            await client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InstallCommandsAsync();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton(_config)
                .BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }
    }
}
