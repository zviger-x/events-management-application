using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Shared.Configuration;
using Shared.Extensions;
using System.Reflection;

namespace Gateway
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;
            var configuration = builder.Configuration;
            var logging = builder.Logging;

            // Add configs
            configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
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
            services.AddOcelot();
            services.AddSwaggerForOcelot(configuration);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Gateway API",
                    Version = "v1",
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                // app.UseSwagger();
                // app.UseSwaggerUI();
                app.UseSwaggerForOcelotUI(options =>
                {
                    options.PathToSwaggerGenerator = "/swagger/docs";
                });
            }

            // app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            await app.UseOcelot();

            app.Run();
        }
    }
}
