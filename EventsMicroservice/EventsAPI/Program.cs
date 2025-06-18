using Application.UnitOfWork.Interfaces;
using EventsAPI.Configuration;
using EventsAPI.Extensions;
using Hangfire;
using Infrastructure.BackgroundJobs;
using Infrastructure.BackgroundJobs.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.UnitOfWork;
using MediatR;
using Shared.Configuration;
using Shared.Extensions;
using Shared.Hangfire.Filters;
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
                .AddJsonFile("/app/config/elk-stack-settings.json", optional: true)
                .AddEnvironmentVariables();
            var redisServerConfig = services.ConfigureAndReceive<RedisServerConfig>(configuration, "Caching:RedisServerConfig");
            var cacheConfig = services.ConfigureAndReceive<CacheConfig>(configuration, "Caching:Cache");
            var mongoServerConfig = services.ConfigureAndReceive<MongoServerConfig>(configuration, "MongoServerConfig");
            var jwtTokenConfig = services.ConfigureAndReceive<JwtTokenConfig>(configuration, "JwtConfig");
            var grpcConnections = services.ConfigureAndReceive<GrpcConnectionsConfig>(configuration, "GrpcConnections");
            var kafkaServerConfig = services.ConfigureAndReceive<KafkaServerConfig>(configuration, "KafkaServerConfig");
            var elkConfig = services.ConfigureAndReceive<ELKConfig>(configuration, "ELKConfig");
            var hangfireConfig = services.ConfigureAndReceive<HangfireConfig>(configuration, "HangfireConfig");

            // Add logging
            logging.ConfigureLogger(
                microserviceName: Assembly.GetExecutingAssembly().GetName().Name,
                writeToLogstash: true,
                logstashUri: elkConfig.LogstashUri,
                logstashMinimumLevel: elkConfig.MinimumLevel);

            // Caching
            services.AddRedisServer(redisServerConfig);
            services.AddCachingServices();

            // Hangfire
            services.AddHangfire(config => config.UseSqlServerStorage(hangfireConfig.ConnectionString));
            services.AddHangfireServer();
            services.AddScoped<INotifyCompletedEventsJob, NotifyCompletedEventsJob>();
            services.AddScoped<INotifyUpcomingEventsJob, NotifyUpcomingEventsJob>();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = [new HangfireAllowAllAuthorizationFilter()]
            });
            app.UseHangfireRecurringJobs(hangfireConfig);

            app.Run();
        }
    }
}
