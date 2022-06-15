using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;
using Serilog.Events;

namespace Echo.Handlers
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services, ILogger logger)
        {
            _client = client;
            _handler = handler;
            _services = services;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _client.Ready += ReadyAsync;
            _client.GuildAvailable += GuildAvailable;
            _handler.Log += LogAsync;

            await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteraction;
        }

        private async Task GuildAvailable(SocketGuild arg)
        {
            await _handler.RegisterCommandsToGuildAsync(arg.Id);
        }

        private async Task ReadyAsync()
        {
            //if (Debugger.IsAttached)
            //await _handler.RegisterCommandsToGuildAsync(827929281899790336); // TODO
            //else
            //    await _handler.RegisterCommandsGloballyAsync();
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(_client, interaction);

                var result = await _handler.ExecuteCommandAsync(context, _services);

                if (!result.IsSuccess)
                {
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            // implement
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Task LogAsync(LogMessage msg)
        {
            LogEventLevel level = msg.Severity switch
            {
                LogSeverity.Critical => LogEventLevel.Fatal,
                LogSeverity.Error => LogEventLevel.Error,
                LogSeverity.Warning => LogEventLevel.Warning,
                LogSeverity.Info => LogEventLevel.Information,
                LogSeverity.Verbose => LogEventLevel.Verbose,
                LogSeverity.Debug => LogEventLevel.Debug,
                _ => throw new ArgumentOutOfRangeException()
            };

            _logger.Write(level, msg.Exception, msg.Message);

            return Task.CompletedTask;
        }
    }
}
