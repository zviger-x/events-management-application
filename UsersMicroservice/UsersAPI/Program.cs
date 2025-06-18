using DataAccess.Contexts;
using DataAccess.Initialization;
using DataAccess.UnitOfWork;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Configuration;
using Shared.Extensions;
using System.Reflection;
using UsersAPI.Configuration;
using UsersAPI.Extensions;
using UsersAPI.Middlewares;
using UsersAPI.Services;

namespace UsersAPI
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
                .AddJsonFile("/app/config/elk-stack-settings.json", optional: true)
                .AddEnvironmentVariables();
            var jwtConfig = services.ConfigureAndReceive<JwtTokenConfig>(configuration, "Jwt");
            var cacheConfig = services.ConfigureAndReceive<CacheConfig>(configuration, "Caching:Cache");
            var redisConfig = services.ConfigureAndReceive<RedisServerConfig>(configuration, "Caching:RedisServerConfig");
            var sqlConfig = services.ConfigureAndReceive<SqlServerConfig>(configuration, "SqlServerConfig");
            var elkConfig = services.ConfigureAndReceive<ELKConfig>(configuration, "ELKConfig");

            // Add logging
            logging.ConfigureLogger(
                microserviceName: Assembly.GetExecutingAssembly().GetName().Name,
                writeToLogstash: true,
                logstashUri: elkConfig.LogstashUri,
                logstashMinimumLevel: elkConfig.MinimumLevel);

            // Redis
            services.AddRedisServer(redisConfig);

            // DAL
            services.AddUserDbContext(sqlConfig);
            services.AddRepositories();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // BLL
            services.AddAutoMapper(Assembly.Load("BusinessLogic"), Assembly.Load("UsersAPI"));
            services.AddValidators();
            services.AddServices();
            services.AddCachingServices();

            // JWT
            services.AddJwtAuthentication(jwtConfig);
            services.AddAuthorization();

            // API
            services.AddGrpcWithInterceptors();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwagger(true, 1);

            var app = builder.Build();

            // Middlewares
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Initializing DB
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

                // Auto migrations
                if (dbContext.Database.GetPendingMigrations().Any())
                    dbContext.Database.Migrate();

                // Regenerate db and seed demo data
                // TODO: Удалить, когда будет установлен дефолтный пользователь с правами админа
                var dbInitializer = new DBInitializer(dbContext, sqlConfig.RecreateDatabase, sqlConfig.SeedDemoData);
                dbInitializer.Initialize();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGrpcService<UserService>();

            app.Run();
        }
    }
}
