using DataAccess.Repositories.Interfaces;
using DataAccess.Repositories;
using DataAccess.Entities;
using BusinessLogic.Validation.Validators.Interfaces;
using BusinessLogic.Validation.Validators;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Services;

namespace UsersAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IRepository<UserNotification>, UserNotificationRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();

            services.AddScoped<IRepository<UserTransaction>, UserTransactionRepository>();
            services.AddScoped<IUserTransactionRepository, UserTransactionRepository>();
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
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();
            services.AddScoped<IUserTransactionService, UserTransactionService>();

            services.AddScoped<IPasswordHashingService, PasswordHashingService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
