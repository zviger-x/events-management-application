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

            // DAL
            var sqlConfig = builder.Configuration.GetSection("SqlServerConfig").Get<SqlServerConfig>();
            if (sqlConfig == null)
                throw new ArgumentNullException(nameof(sqlConfig));

            services.AddDbContext<UserDbContext>(o => o.UseSqlServer(sqlConfig.ConnectionString));
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            var app = builder.Build();

            // Initializing DB
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                var dbInitializer = new DBInitializer(dbContext, sqlConfig);
                dbInitializer.Initialize();
            }

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
