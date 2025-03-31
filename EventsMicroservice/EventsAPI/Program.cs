using Application.Mapping;
using Application.UnitOfWork.Interfaces;
using EventsAPI.Configuration;
using EventsAPI.Extensions;
using EventsAPI.Middlewares;
using Infrastructure.Contexts;
using Infrastructure.UnitOfWork;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace EventsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;

            // Add configs

            // Add logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            // Redis

            // Data access
            var mongoConfig = builder.Configuration.GetSection("MongoServerConfig").Get<MongoServerConfig>();
            if (mongoConfig == null)
                throw new ArgumentNullException(nameof(mongoConfig));
            services.AddMongoServer(mongoConfig);
            services.AddScoped<EventDbContext>();
            services.AddRepositories();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Use cases
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddValidators();
            services.AddUseCases();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
