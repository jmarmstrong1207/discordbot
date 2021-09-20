using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace CritzlezOS
{
    public class MainBot
    {
        public readonly string token;

        public SocketCommandContext Context { get; set; }
        public SocketUserMessage Message { get; set; }
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public ulong MessageID { get; set; }
        public string LogMessage { get; set; }    

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public MainBot()
        {
            token = System.IO.File.ReadAllText("token.txt");
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            if (!(messageParam is SocketUserMessage message)) return;

            // Create a Command Context
            var context = new SocketCommandContext(_client, message);

            // Gets info of message
            Context = context;
            MessageID = message.Id;
            Message = message;
            LogMessage = message.Content;

            // Adds image attachment urls to log if they're in the message
            if (message.Attachments.Count != 0)
            {
                foreach (Attachment a in message.Attachments)
                {
                    LogMessage += "\n" + a.Url;
                }
            }

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix(',', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += HandleCommandAsync;

            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            await _client.SetGameAsync("asynchronously");

            await InstallCommandsAsync();
        }

        public async Task StopAsync()
        {
            await _client.StopAsync();

            // Wait a little for the client to finish disconnecting before allowing the program to return
            await Task.Delay(500);
        }
    }

    public struct BotInherited
    {
        public static MainBot bot = new MainBot();
    }
}
