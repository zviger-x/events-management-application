using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Configuration;
using Shared.Swagger.Filters;
using System.Text;

namespace Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds JWT Bearer authentication using the specified token configuration.
        /// </summary>
        /// <param name="services">The service collection to register authentication services with.</param>
        /// <param name="config">The JWT token configuration containing issuer, audience, and secret key.</param>
        public static void AddJwtAuthentication(this IServiceCollection services, JwtTokenConfig config)
        {

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config.Issuer,
                        ValidAudience = config.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        /// <summary>
        /// Adds and configures Swagger generation.
        /// </summary>
        /// <param name="services">The service collection to register Swagger services with.</param>
        /// <param name="useRouteGrouping">
        /// Indicates whether to enable route grouping based on controller paths.
        /// If true, applies a grouping filter on a portion of the route path.
        /// </param>
        /// <param name="routeWordOffset">Used to offset the index of the word in the route by which it will be grouped</param>
        public static void AddSwagger(this IServiceCollection services, bool useRouteGrouping = false, int routeWordOffset = 0)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
                options.OperationFilter<RolesOperationFilter>();

                if (useRouteGrouping)
                    options.OperationFilter<RouteGroupingOperationFilter>(routeWordOffset);
            });
        }

        /// <summary>
        /// Binds a configuration section to a strongly-typed configuration class, 
        /// registers it with the service collection, and returns the configured instance.
        /// </summary>
        /// <typeparam name="TConfig">The type of the configuration class to bind to.</typeparam>
        /// <param name="services">The service collection to register the configuration with.</param>
        /// <param name="configuration">The application's configuration root.</param>
        /// <param name="sectionName">The name of the configuration section to bind.</param>
        /// <returns>The registered configuration instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the specified configuration section is not found or the configuration object could not be created.
        /// </exception>
        public static TConfig ConfigureAndReceive<TConfig>(this IServiceCollection services, IConfiguration configuration, string sectionName)
            where TConfig : class, new()
        {
            var section = configuration.GetSection(sectionName);

            if (!section.Exists())
                throw new InvalidOperationException($"Configuration section '{sectionName}' not found.");

            var config = section.Get<TConfig>();
            if (config == null)
                throw new InvalidOperationException($"Failed to create configuration object for section '{sectionName}'.");

            services.Configure<TConfig>(section);
            return config;
        }
    }
}
