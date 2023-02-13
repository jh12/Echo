using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Echo.Config;
using Echo.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace Echo
{
    class Program
    {
        private static ILogger _logger = null!;

        private static readonly IServiceProvider Services = ServiceProviderConfig.Create();

        static async Task Main(string[] args)
        {
            _logger = Services.GetRequiredService<ILogger>();

            IOptions<DiscordOptions> discordOptions = Services.GetRequiredService<IOptions<DiscordOptions>>();
            DiscordSocketClient client = Services.GetRequiredService<DiscordSocketClient>();

            client.Log += Log;

            await Services.GetRequiredService<InteractionHandler>()
                .InitializeAsync();

            await client.LoginAsync(TokenType.Bot, discordOptions.Value.Token);
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private static Task Log(LogMessage msg)
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
