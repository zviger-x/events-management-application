using Application.Clients;
using Application.Services.Background;
using Application.SignalR;
using Infrastructure.Clients;
using Infrastructure.Services.Background;
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
            services.AddHostedService<StubNotificationSenderService>();

            // API
            services.AddSignalR();

            var app = builder.Build();

            app.MapHub<NotificationHub>("/notifications");

            app.Run();
        }
    }
}
