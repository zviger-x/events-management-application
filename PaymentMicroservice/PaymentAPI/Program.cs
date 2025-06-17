using MediatR;
using PaymentAPI.Extensions;
using PaymentAPI.Services;
using Serilog;
using Shared.Logging;

namespace PaymentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;
            var configuration = builder.Configuration;
            var logging = builder.Logging;

            // Add logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: CustomConsoleThemes.SixteenEnhanced)
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
            logging.ClearProviders();
            logging.AddSerilog();

            services.AddAutoMapper();

            // BLL
            services.AddClients();
            services.AddValidators();
            services.AddSagas();
            services.AddMediatR();

            // API
            services.AddGrpcWithInterceptors();
            services.AddGrpcSwagger();
            services.AddSwagger();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapGrpcService<PaymentService>();

            app.Run();
        }
    }
}
