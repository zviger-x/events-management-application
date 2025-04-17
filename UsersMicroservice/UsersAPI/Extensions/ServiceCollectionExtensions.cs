using BusinessLogic.Caching;
using BusinessLogic.Caching.Interfaces;
using BusinessLogic.Configuration;
using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using UsersAPI.Configuration;

namespace UsersAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRedisServer(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection("RedisServerConfig").Get<RedisServerConfig>();
            if (redisConfig == null)
                throw new ArgumentNullException(nameof(redisConfig));

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig.ConnectionString));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig.ConnectionString;
                options.InstanceName = redisConfig.CachePrefix;
            });
        }

        public static void AddUserDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConfig = configuration.GetSection("SqlServerConfig").Get<SqlServerConfig>();
            if (sqlConfig == null)
                throw new ArgumentNullException(nameof(sqlConfig));

            services.AddDbContext<UserDbContext>(o =>
            {
                o.UseSqlServer(sqlConfig.ConnectionString);
                o.EnableSensitiveDataLogging();
            });
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
            services.AddScoped<IUserValidator, UserValidator>();
            services.AddScoped<IUserNotificationValidator, UserNotificationValidator>();
            services.AddScoped<IUserTransactionValidator, UserTransactionValidator>();

            services.AddScoped<IUpdateUserDTOValidator, UpdateUserDTOValidator>();
            services.AddScoped<IChangePasswordDTOValidator, ChangePasswordDTOValidator>();
            services.AddScoped<IRegisterDTOValidator, RegisterDTOValidator>();
            services.AddScoped<ILoginDTOValidator, LoginDTOValidator>();
            services.AddScoped<IChangeUserRoleDTOValidator, ChangeUserRoleDTOValidator>();
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

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            var jwtConfig = configuration.GetSection("Jwt").Get<JwtTokenConfig>();
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

        public static void AddSwagger(this IServiceCollection services)
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

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
