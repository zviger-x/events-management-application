using BusinessLogic.Configuration;
using BusinessLogic.Mapping;
using DataAccess.Contexts;
using DataAccess.Initialization;
using DataAccess.UnitOfWork;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using UsersAPI.Configuration;
using UsersAPI.Extensions;
using UsersAPI.Middlewares;

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

            // Add configs
            var jwtConfig = services.ConfigureAndReceive<JwtTokenConfig>(configuration, "Jwt");
            var cacheConfig = services.ConfigureAndReceive<CacheConfig>(configuration, "Caching:Cache");
            var redisConfig = services.ConfigureAndReceive<RedisServerConfig>(configuration, "Caching:RedisServerConfig");
            var sqlConfig = services.ConfigureAndReceive<SqlServerConfig>(configuration, "SqlServerConfig");

            // Add logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            // Redis
            services.AddRedisServer(redisConfig);

            // DAL
            services.AddUserDbContext(sqlConfig);
            services.AddRepositories();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // BLL
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddValidators();
            services.AddServices();
            services.AddCachingServices();

            // JWT
            services.AddJwtAuthentication(jwtConfig);
            services.AddAuthorization();

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

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
