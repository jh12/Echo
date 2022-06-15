using System;
using Discord.Interactions;
using Discord.WebSocket;
using Echo.DataAccess.Repositories;
using Echo.Domain.Repositories;
using Echo.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Echo.Config
{
    public static class ServiceProviderConfig
    {
        public static IServiceProvider Create()
        {
            return new ServiceCollection()
                .AddSocketConfig()
                .AddLogger()
                .AddConfiguration()
                .AddRepositories()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .BuildServiceProvider();
        }

        private static ServiceCollection AddSocketConfig(this ServiceCollection services)
        {
            //DiscordSocketConfig socketConfig = new DiscordSocketConfig
            //{
            //    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            //    AlwaysDownloadUsers = true
            //};

            //services.AddSingleton(socketConfig);

            return services;
        }

        private static ServiceCollection AddLogger(this ServiceCollection services)
        {
            LoggerConfiguration configuration = new LoggerConfiguration();
            configuration.MinimumLevel.Verbose()
                .WriteTo.Console();

            services.AddSingleton<ILogger>(configuration.CreateLogger());

            return services;
        }

        private static ServiceCollection AddConfiguration(this ServiceCollection services)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddJsonFile("appsettings.json", true, false)
                .AddEnvironmentVariables();

            IConfigurationRoot configurationRoot = configurationBuilder.Build();

            services.AddOptions();

            services.AddOptions<DiscordOptions>().Bind(configurationRoot.GetRequiredSection("Discord"));

            return services;
        }

        private static ServiceCollection AddRepositories(this ServiceCollection services)
        {
            services.AddSingleton<IDiceRepository, DiceRepository>();

            return services;
        }
    }
}
