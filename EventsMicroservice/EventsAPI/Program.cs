using Application.Mapping;
using Application.UnitOfWork.Interfaces;
using EventsAPI.Configuration;
using EventsAPI.Extensions;
using Infrastructure.Contexts;
using Infrastructure.UnitOfWork;
using MediatR;
using Serilog;
using Shared.Configuration;
using Shared.Extensions;
using Shared.Logging;
using Shared.Middlewares;

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
            var redisServerConfig = services.ConfigureAndReceive<RedisServerConfig>(configuration, "Caching:RedisServerConfig");
            var cacheConfig = services.ConfigureAndReceive<CacheConfig>(configuration, "Caching:Cache");
            var mongoServerConfig = services.ConfigureAndReceive<MongoServerConfig>(configuration, "MongoServerConfig");
            var jwtTokenConfig = services.ConfigureAndReceive<JwtTokenConfig>(configuration, "JwtConfig");

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

            // Data access
            services.AddMongoServer(mongoServerConfig);
            services.AddScoped<TransactionContext>();
            services.AddScoped<EventDbContext>();
            services.AddRepositories();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Business logic
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddClients();
            services.AddValidators();
            services.AddMediatR();

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

            app.Run();
        }
    }
}
