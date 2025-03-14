using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Contexts;
using DataAccess.Initialization;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace UsersAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;

            // Для удобства, пока доступен открытый порт
            // Я в терминале указываю useSqlOpenPorts, чтобы при миграциях использовался открытый порт
            // А само приложение работало на закрытых портах
            var openSqlPorts = args.Contains("UseSqlOpenPorts");

            var sqlConfig = builder.Configuration.GetSection("SqlServerConfig").Get<SqlServerConfig>();
            if (sqlConfig == null)
                throw new ArgumentNullException(nameof(sqlConfig));

            // DAL
            services.AddDbContext<UserDbContext>(o =>
                o.UseSqlServer(openSqlPorts ? sqlConfig.ConnectionStringOpenPorts : sqlConfig.ConnectionString));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<IUserTransactionRepository, UserTransactionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWOrk>();

            // BLL
            services.AddScoped<IUserValidator, UserValidator>();
            services.AddScoped<IUserNotificationValidator, UserNotificationValidator>();
            services.AddScoped<IUserTransactionValidator, UserTransactionValidator>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();
            services.AddScoped<IUserTransactionService, UserTransactionService>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            var app = builder.Build();

            // Initializing DB
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

                // Auto migrations
                if (dbContext.Database.GetPendingMigrations().Any())
                    dbContext.Database.Migrate();
            
                // Regenerate db and seed demo data
                var dbInitializer = new DBInitializer(dbContext, sqlConfig);
                dbInitializer.Initialize();
            }

            // gRPC Swagger
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
