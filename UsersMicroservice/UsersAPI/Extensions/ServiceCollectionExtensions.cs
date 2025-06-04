using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;
using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Caching.Services;
using Shared.Caching.Services.Interfaces;
using Shared.Grpc.Interceptors;
using Shared.Repositories.Interfaces;
using StackExchange.Redis;
using System.Reflection;
using UsersAPI.Configuration;

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
            services.AddValidatorsFromAssembly(Assembly.Load("Shared"));
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

        public static void AddGrpcWithInterceptors(this IServiceCollection services)
        {
            services.AddGrpc(o =>
            {
                o.Interceptors.Add<GrpcExceptionInterceptor>();
            });
        }
    }
}
