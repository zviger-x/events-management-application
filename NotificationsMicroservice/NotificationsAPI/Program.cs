using Application.Clients;
using Application.Services.Background;
using Application.SignalR;
using Infrastructure.Clients;
using Infrastructure.Kafka.MessageHandlers;
using Infrastructure.Kafka.MessageHandlers.Interfaces;
using Infrastructure.Kafka.Services;
using NotificationsAPI.Configuration;
using NotificationsAPI.Extensions;
using NotificationsAPI.SignalR.Hubs;
using NotificationsAPI.SignalR.Senders;
using Serilog;
using Shared.Configuration;
using Shared.Extensions;
using Shared.Logging;

namespace NotificationsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;
            var configuration = builder.Configuration;
            var logging = builder.Logging;

            // Add configs
            var redisServerConfig = services.ConfigureAndReceive<RedisServerConfig>(configuration, "Caching:RedisServerConfig");
            var cacheConfig = services.ConfigureAndReceive<CacheConfig>(configuration, "Caching:Cache");
            var jwtTokenConfig = services.ConfigureAndReceive<JwtTokenConfig>(configuration, "JwtConfig");
            var kafkaServerConfig = services.ConfigureAndReceive<KafkaServerConfig>(configuration, "KafkaServerConfig");

            // Add logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: CustomConsoleThemes.SixteenEnhanced)
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
            logging.ClearProviders();
            logging.AddSerilog();

            // Caching
            services.AddRedisServer(redisServerConfig);
            services.AddCachingServices();

            // Business logic
            services.AddMediatR();
            services.AddHostedService<FailedNotificationResender>();
            services.AddScoped<INotificationSender, NotificationSender>();
            services.AddScoped<IUserNotificationsClient, UserNotificationsClientStub>();

            // Infrastructure
            // services.AddHostedService<StubNotificationSenderService>();
            services.AddSingleton<IStubKafkaMessageHandler, StubKafkaMessageHandler>();
            services.AddHostedService<StubKafkaReceiveService>();

            // JWT
            services.AddJwtAuthentication(jwtTokenConfig)
                .ConfigureSignalRTokenHandling("/hubs/notifications");
            services.AddAuthorization();

            // API
            services.AddSignalR();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<NotificationHub>("/hubs/notifications");

            app.Run();
        }
    }
}
