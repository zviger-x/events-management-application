using Shared.Configuration;
using Shared.Extensions;
using System.Reflection;

namespace Gateway
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
            var jwtTokenConfig = services.ConfigureAndReceive<JwtTokenConfig>(configuration, "JwtConfig");
            var elkConfig = services.ConfigureAndReceive<ELKConfig>(configuration, "ELKConfig");

            // Add logging
            logging.ConfigureLogger(
                microserviceName: Assembly.GetExecutingAssembly().GetName().Name,
                writeToLogstash: true,
                logstashUri: elkConfig.LogstashUri,
                logstashMinimumLevel: elkConfig.MinimumLevel);

            // JWT
            services.AddJwtAuthentication(jwtTokenConfig);
            services.AddAuthorization();

            // API
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }
    }
}
