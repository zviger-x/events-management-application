using Application.Mapping;
using Application.UnitOfWork.Interfaces;
using EventsAPI.Configuration;
using EventsAPI.Extensions;
using EventsAPI.Middlewares;
using Infrastructure.Contexts;
using Infrastructure.UnitOfWork;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
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

            // Add logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
            logging.ClearProviders();
            logging.AddSerilog();

            // Redis

            // Data access
            var mongoConfig = builder.Configuration.GetSection("MongoServerConfig").Get<MongoServerConfig>();
            if (mongoConfig == null)
                throw new ArgumentNullException(nameof(mongoConfig));
            services.AddMongoServer(mongoConfig);
            services.AddScoped<TransactionContext>();
            services.AddScoped<EventDbContext>();
            services.AddRepositories();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Use cases (via MediatR)
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddValidators();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));

            // JWT
            services.AddJwtAuthentication(configuration);
            services.AddAuthorization();

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
