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

            // ��� ��������, ���� �������� �������� ����
            // � � ��������� �������� useSqlOpenPorts, ����� ��� ��������� ������������� �������� ����
            // � ���� ���������� �������� �� �������� ������
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

            builder.Services.AddGrpc().AddJsonTranscoding();
            builder.Services.AddGrpcSwagger();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "gRPC Users Microservice", Version = "v1" });
            });

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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

            app.Run();
        }
    }
}
