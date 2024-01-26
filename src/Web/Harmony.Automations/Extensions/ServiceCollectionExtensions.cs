﻿using Harmony.Application.Configurations;
using Harmony.Application.Contracts.Automation;
using Harmony.Application.Contracts.Messaging;
using Harmony.Application.Contracts.Persistence;
using Harmony.Application.Contracts.Services.Identity;
using Harmony.Application.Contracts.Services.Management;
using Harmony.Automations.Contracts;
using Harmony.Automations.Services;
using Harmony.Infrastructure.Mappings;
using Harmony.Infrastructure.Seed;
using Harmony.Infrastructure.Services.Identity;
using Harmony.Infrastructure.Services.Management;
using Harmony.Messaging;
using System.Reflection;

namespace Harmony.Automations.Extensions
{
    /// <summary>
    /// Extension class to register services per category
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBoardService, BoardService>();
            services.AddAutoMapper(typeof(UserProfile).Assembly);

            return services;
        }

        internal static IServiceCollection AddEndpointConfiguration(this IServiceCollection services,
           IConfiguration configuration)
        {
            var endpointConfiguration = configuration
                .GetSection(nameof(AppEndpointConfiguration));

            services.Configure<AppEndpointConfiguration>(endpointConfiguration);

            return services;
        }

        internal static IServiceCollection AddDbSeed(
            this IServiceCollection services)
            => services.AddScoped<IDatabaseSeeder, MongoDbSeeder>();

        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        }

        internal static IServiceCollection AddAutomationServices(this IServiceCollection services)
        {
            services.AddScoped<ICardMovedAutomationService, CardMovedAutomationService>();
            services.AddScoped<ISyncParentAndChildIssuesAutomationService, SyncParentAndChildIssuesAutomationService>();

            return services;
        }

        internal static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbConfiguration>(configuration.GetSection("MongoDb"));

            Persistence.Configurations.MongoDb.MongoDbConfiguration.Configure();

            return services;
        }

        internal static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BrokerConfiguration>(configuration.GetSection("BrokerConfiguration"));

            services.AddSingleton<INotificationsPublisher, RabbitMQNotificationPublisher>();

            return services;
        }
    }
}
