using Application.Services.Background;
using Application.SignalR;
using NotificationsAPI.Configuration;
using NotificationsAPI.Extensions;
using NotificationsAPI.SignalR.Hubs;
using NotificationsAPI.SignalR.Senders;
using Serilog;
using Serilog.Events;
using Shared.Configuration;
using Shared.Extensions;
using Shared.Logging;
using System.Reflection;

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
            configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("/app/config/grpc-connections.json", optional: true)
                .AddJsonFile("/app/config/kafka-server-settings.json", optional: true)
                .AddEnvironmentVariables();
            var redisServerConfig = services.ConfigureAndReceive<RedisServerConfig>(configuration, "Caching:RedisServerConfig");
            var cacheConfig = services.ConfigureAndReceive<CacheConfig>(configuration, "Caching:Cache");
            var jwtTokenConfig = services.ConfigureAndReceive<JwtTokenConfig>(configuration, "JwtConfig");
            var kafkaServerConfig = services.ConfigureAndReceive<KafkaServerConfig>(configuration, "KafkaServerConfig");
            var grpcConnections = services.ConfigureAndReceive<GrpcConnectionsConfig>(configuration, "GrpcConnections");

            // Add logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: CustomConsoleThemes.SixteenEnhanced)
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .WriteTo.Http("http://logstash:8098", null, restrictedToMinimumLevel: LogEventLevel.Warning)
                .CreateLogger();
            logging.ClearProviders();
            logging.AddSerilog();

            // Caching
            services.AddRedisServer(redisServerConfig);
            services.AddCachingServices();

            // Business logic
            services.AddAutoMapper(Assembly.Load("Infrastructure"));
            services.AddMediatR();
            services.AddHostedService<FailedNotificationResender>();
            services.AddScoped<INotificationSender, NotificationSender>();
            services.AddClients(grpcConnections);

            // Infrastructure
            services.AddKafkaMessageHandlers();
            services.AddKafkaBackgroundServices();

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
