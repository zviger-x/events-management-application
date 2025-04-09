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
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, string section)
        {
        
            var jwtConfig = configuration.GetSection(section).Get<JwtTokenConfig>();
            if (jwtConfig == null)
                throw new ArgumentNullException(nameof(jwtConfig));
        
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

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
    }
}
