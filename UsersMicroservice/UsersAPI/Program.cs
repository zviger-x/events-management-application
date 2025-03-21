using BusinessLogic.Configuration;
using BusinessLogic.Mapping;
using DataAccess.Contexts;
using DataAccess.Initialization;
using DataAccess.UnitOfWork;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System.Text;
using UsersAPI.Configuration;
using UsersAPI.Extensions;
using UsersAPI.Middlewares;

namespace UsersAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;

            // Add logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            // Redis
            var redisConfig = builder.Configuration.GetSection("RedisServerConfig").Get<RedisServerConfig>();
            if (redisConfig == null) 
                throw new ArgumentNullException(nameof(redisConfig));
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig.ConnectionString));
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig.ConnectionString;
                options.InstanceName = redisConfig.CachePrefix;
            });

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
            services.AddRepositories();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // BLL
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddValidators();
            services.AddServices();
            services.AddCachingServices();

            // JWT
            var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtTokenConfig>();
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
            services.AddAuthorization();
            
            services.AddControllers();
            services.AddEndpointsApiExplorer();
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

            var app = builder.Build();

            // Middlewares
            app.UseMiddleware<ExceptionHandlingMiddleware>();

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

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
