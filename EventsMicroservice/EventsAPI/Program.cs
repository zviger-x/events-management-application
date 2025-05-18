using Application.UnitOfWork.Interfaces;
using EventsAPI.Configuration;
using EventsAPI.Extensions;
using Hangfire;
using Infrastructure.BackgroundJobs;
using Infrastructure.BackgroundJobs.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.UnitOfWork;
using MediatR;
using Serilog;
using Shared.Configuration;
using Shared.Extensions;
using Shared.Logging;
using Shared.Middlewares;
using System.Reflection;

namespace EventsAPI
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
            var mongoServerConfig = services.ConfigureAndReceive<MongoServerConfig>(configuration, "MongoServerConfig");
            var jwtTokenConfig = services.ConfigureAndReceive<JwtTokenConfig>(configuration, "JwtConfig");
            var grpcConnections = services.ConfigureAndReceive<GrpcConnectionsConfig>(configuration, "GrpcConnections");
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

            // Hangfire
            services.AddHangfire(config =>
                config.UseSqlServerStorage(configuration.GetSection("HangfireConfig:ConnectionString").Value));
            services.AddHangfireServer();
            services.AddScoped<INotifyCompletedEventsJob, NotifyCompletedEventsJob>();

            // Data access
            services.AddMongoServer(mongoServerConfig);
            services.AddScoped<TransactionContext>();
            services.AddScoped<EventDbContext>();
            services.AddRepositories();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Business logic
            services.AddAutoMapper(Assembly.Load("Application"), Assembly.Load("Infrastructure"));
            services.AddClients(grpcConnections);
            services.AddValidators();
            services.AddMediatR();
            services.AddKafkaProducers();

            // JWT
            services.AddJwtAuthentication(jwtTokenConfig);
            services.AddAuthorization();

            // API
            services.AddGrpcWithInterceptors();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwagger(true, 1);

            var app = builder.Build();

            // Middlewares
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseHangfireDashboard("/hangfire");
            app.UseHangfireRecurringJobs();

            app.Run();
        }
    }
}
