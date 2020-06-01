using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using BachUZ.Database;
using BachUZ.Events;
using BachUZ.Services;


namespace BachUZ.Discord
{
    public class Program
    {
        public static ConcurrentDictionary<ulong, string> CustomPrefixes = new ConcurrentDictionary<ulong, string>();
        private IConfiguration _config = null!;

        public static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public async Task MainAsync(string[] args)
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

            await CreateHostBuilder(args).Build().RunAsync();
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
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
