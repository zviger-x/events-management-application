using Application.Clients;
using Infrastructure.Clients.Grpc;
using Infrastructure.Kafka.MessageHandlers;
using Infrastructure.Kafka.MessageHandlers.Interfaces;
using Infrastructure.Kafka.Services;
using NotificationsAPI.Configuration;
using Shared.Caching.Services;
using Shared.Caching.Services.Interfaces;
using Shared.Configuration;
using Shared.Grpc.User;
using StackExchange.Redis;
using System.Reflection;

namespace NotificationsAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));
        }

        public static void AddRedisServer(this IServiceCollection services, RedisServerConfig config)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(config.ConnectionString));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.ConnectionString;
                options.InstanceName = config.CachePrefix;
            });
        }

        public static void AddCachingServices(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
        }

        public static void AddKafkaMessageHandlers(this IServiceCollection services)
        {
            services.AddSingleton<IEventUpcomingMessageHandler, EventUpcomingMessageHandler>();
            services.AddSingleton<IEventUpdatedMessageHandler, EventUpdatedMessageHandler>();
            services.AddSingleton<IEventCompletedMessageHandler, EventCompletedMessageHandler>();
            services.AddSingleton<IPaymentConfirmedMessageHandler, PaymentConfirmedMessageHandler>();
        }

        public static void AddKafkaBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<EventUpcomingKafkaReceiveService>();
            services.AddHostedService<EventUpdatedKafkaReceiveService>();
            services.AddHostedService<EventCompletedKafkaReceiveService>();
            services.AddHostedService<PaymentConfirmedKafkaReceiveService>();
        }

        public static void AddClients(this IServiceCollection services, GrpcConnectionsConfig grpcConnections)
        {
            var httpHandler = () => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            services.AddGrpcClient<UserService.UserServiceClient>(o =>
            {
                o.Address = new Uri(grpcConnections.UsersMicroservice);
            })
            .ConfigurePrimaryHttpMessageHandler(httpHandler);

            services.AddScoped<IUserNotificationsClient, UserNotificationsClient>();
        }
    }
}
