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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BachUZ
{
    class Program
    {
        public static ConcurrentDictionary<ulong, string> CustomPrefixes = new ConcurrentDictionary<ulong, string>();
        private IConfiguration _config = null!;
        private static readonly HttpListener Listener = new HttpListener();
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

            //http response handler
            await Task.Run(async () =>
            {
                try
                {
                    Listener.Start();
                }
                catch (HttpListenerException hlex)
                {
                    Console.Error.WriteLine(hlex.Message);
                    return;
                }

                while (true)
                {
                    var context = await Listener.GetContextAsync();
                    var request = context.Request;
                    var response = context.Response;
                    context.Response.Close();
                }
            });
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
