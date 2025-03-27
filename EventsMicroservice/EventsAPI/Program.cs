using Application.UnitOfWork.Interfaces;
using EventsAPI.Extensions;
using Infrastructure.UnitOfWork;

namespace EventsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;

            // Add configs

            // Add logging

            // Redis

            // Data access
            #warning TODO: Добавить подключение к монго
            services.AddRepositories();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Use cases
            #warning TODO: Добавить профиль мапперы
            services.AddValidators();
            services.AddUseCases();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            var app = builder.Build();

            // Middlewares
            app.UseMiddleware<ExceptionHandlingMiddleware>();

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
