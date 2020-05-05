using BachUZ.Database;
using BachUZ.Events;
using BachUZ.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BachUZ
{
    class Program
    {
        public static ConcurrentDictionary<ulong, string> CustomPrefixes = new ConcurrentDictionary<ulong, string>();
        private IConfiguration _config = null!;
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _config = BuildConfig();

            await using (var database = new BachuzContext())
            {
                foreach (var guild in database.Guilds.AsQueryable().Select(item => item))
                {
                    if (!string.IsNullOrEmpty(guild.CustomPrefix))
                    {
                        CustomPrefixes.TryAdd(Convert.ToUInt64(guild.GuildId), guild.CustomPrefix);
                    }
                }
            }

            await using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;
            await client.LoginAsync(TokenType.Bot, _config["token"]);
            await client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InstallCommandsAsync();
            client.JoinedGuild += JoinedGuild.HandleEvent;
            client.LeftGuild += LeftGuild.HandleEvent;
            client.GuildUpdated += GuildUpdated.HandleEvent;
            client.MessageReceived += MessageReceived.HandleEvent;
            client.UserVoiceStateUpdated += UserVoiceStateUpdated.HandleEvent;
            client.Ready += () =>
            {
                client.SetActivityAsync(new Game($"Default prefix: {_config["prefix"]}"));
                return Task.CompletedTask;
            };

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
                .AddSingleton(new DiscordSocketClient(
                    new DiscordSocketConfig
                    {
                        MessageCacheSize = 100
                    }))
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton(_config)
                .AddTransient<BachuzContext>()
                .BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
                .AddJsonFile("config.json")
                .Build();
        }
    }
}
