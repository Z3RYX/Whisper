using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

// Initiates the bot and starts it
var bot = new Whisper.Bot();
bot.MainAsync().GetAwaiter().GetResult();

namespace Whisper
{
    public class Bot
    {
        private DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Services;

        /// <summary>
        /// The main method of Whisper, which handles most of the logic
        /// </summary>
        public async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                GatewayIntents =
                    GatewayIntents.Guilds |
                    GatewayIntents.GuildMembers |
                    GatewayIntents.GuildBans |
                    GatewayIntents.GuildInvites |
                    GatewayIntents.GuildMessages |
                    GatewayIntents.DirectMessages
            });

            Commands = new CommandService();
            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            Config.Client = Client;
            Config.Commands = Commands;

            try
            {
                // Load config file
                Config.Load();
            }
            catch (FileNotFoundException)
            {
                // If config file does not exist, tell the user and generate default config file
                Console.WriteLine("Config File not found. One with default values has been created, go fill it out and restart the bot.");
                File.WriteAllText("./config.json", JsonConvert.SerializeObject(new CfgFile()));

                // End program
                Environment.Exit(0);
            }

            await Commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: Services);

            // Default Events
            Client.Log += Log;
            Client.Ready += OnReady;
            Client.MessageReceived += OnMessage;
        }

        /// <summary>
        /// Fires when the bot is connected and ready to be used
        /// </summary>
        private async Task OnReady()
        {
            // Show the bot you're connected to
            await Log(new LogMessage(LogSeverity.Info, "Ready", $"Connected to: {Client.CurrentUser} ({Client.CurrentUser.Id})"));

            // Set Activity Message
            await SetActivityString();
        }

        /// <summary>
        /// Logs a message to Console and Database
        /// </summary>
        /// <param name="arg">Message to be logged</param>
        private Task Log(LogMessage arg)
        {
            // Output log message to console
            Console.WriteLine($"[{DateTime.UtcNow}: {arg.Source}] {arg.Severity} | {arg.Message}");

            // TODO Store log message in database

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles messages as they are received
        /// </summary>
        /// <param name="arg">Message that was received</param>
        private async Task OnMessage(SocketMessage arg)
        {
            // Check if message was sent by a user
            if (!(arg is SocketUserMessage message)) return;

            // Continue to handle the message
            await HandleMessage(message);
        }

        /// <summary>
        /// Handles user messages
        /// </summary>
        /// <param name="message">Message sent by a user</param>
        private async Task HandleMessage(SocketUserMessage message)
        {
            // Context of the message
            var context = new SocketCommandContext(Client, message);

            // TODO handle commands

            return;
        }

        /// <summary>
        /// Sets the activity to display guild count, activity message, and prefix
        /// </summary>
        private async Task SetActivityString()
            => await Client.SetActivityAsync(new Game($"over {Client.Guilds.Count} Guilds{(Config.ActivityMessage == "" ? "" : " | " + Config.ActivityMessage)} | {Config.Prefix}help", ActivityType.Watching));
    }

    /// <summary>
    /// Structure of the Config File
    /// </summary>
    public static class Config
    {
        // Default Prefix
        public static string Prefix { get; set; }

        // The bot's token
        public static string Token { get; set; }

        // Discord Client
        public static DiscordSocketClient Client { get; set; }

        // Command Service
        public static CommandService Commands { get; set; }

        // Start time
        public static DateTime Startup { get; set; }

        // Colors for embeds
        public static Dictionary<string, Color> ColorScheme { get; set; }

        // The message in the bot's custom status
        public static string ActivityMessage { get; set; }

        // Mongo Database
        public static IMongoDatabase DB { get; set; }

        /// <summary>
        /// Loads the config file
        /// </summary>
        public static void Load()
        {
            var cfg = JsonConvert.DeserializeObject<CfgFile>(File.ReadAllText("./config.json"));

            Prefix = cfg.prefix;
            Token = cfg.token;

            DB = new MongoClient(cfg.mongo_url).GetDatabase(cfg.database_name);

            // Sets the color scheme for all sorts of embeds
            ColorScheme = new Dictionary<string, Color>();
            ColorScheme
                .Add("default", new Color(59, 255, 196));

            Startup = DateTime.UtcNow;
            ActivityMessage = "";
        }
    }

    internal class CfgFile
    {
        [JsonProperty("token")]
        internal string token;
        [JsonProperty("prefix")]
        internal string prefix;
        [JsonProperty("mongo_url")]
        internal string mongo_url;
        [JsonProperty("database_name")]
        internal string database_name;
    }
}
