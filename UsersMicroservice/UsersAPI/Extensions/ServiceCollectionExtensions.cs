using BusinessLogic.Caching;
using BusinessLogic.Caching.Interfaces;
using BusinessLogic.Configuration;
using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;
using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Grpc.Interceptors;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using UsersAPI.Configuration;
using UsersAPI.Swagger.Filters;

namespace UsersAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRedisServer(this IServiceCollection services, RedisServerConfig redisConfig)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig.ConnectionString));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig.ConnectionString;
                options.InstanceName = redisConfig.CachePrefix;
            });
        }

        public static void AddUserDbContext(this IServiceCollection services, SqlServerConfig sqlConfig)
        {
            services.AddDbContext<UserDbContext>(o => o.UseSqlServer(sqlConfig.ConnectionString));
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IRepository<UserNotification>, UserNotificationRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

            services.AddScoped<IRepository<UserTransaction>, UserTransactionRepository>();
            services.AddScoped<IUserTransactionRepository, UserTransactionRepository>();

            services.AddScoped<IRepository<RefreshToken>, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        }

        public static void AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.Load("BusinessLogic"));
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();
            services.AddScoped<IUserTransactionService, UserTransactionService>();

            services.AddScoped<IPasswordHashingService, PasswordHashingService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
        }

        public static void AddCachingServices(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, RedisCacheService>();
        }

        public static void AddJwtAuthentication(this IServiceCollection services, JwtTokenConfig jwtConfig)
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

        public static void AddGrpcWithInterceptors(this IServiceCollection services)
        {
            services.AddGrpc(o =>
            {
                o.Interceptors.Add<GrpcExceptionInterceptor>();
            });
        }
    }
}
