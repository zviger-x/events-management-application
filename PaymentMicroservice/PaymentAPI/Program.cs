using Application.Clients;
using Application.MediatR.Behaviours;
using FluentValidation;
using Infrastructure.Clients;
using MediatR;
using PaymentAPI.Services;
using Serilog;
using Shared.Grpc.Interceptors;
using Shared.Logging;
using System.Reflection;

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

            services.AddAutoMapper([
                Assembly.Load("Application"),
                Assembly.Load("Infrastructure"),
                Assembly.Load("PaymentAPI")]);

            // BLL
            services.AddScoped<IUserClient, UserClientStub>();
            services.AddScoped<IPaymentClient, PaymentClientStub>();
            services.AddValidatorsFromAssembly(Assembly.Load("Infrastructure"));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // API
            services.AddGrpc(o => o.Interceptors.Add<GrpcExceptionInterceptor>()).AddJsonTranscoding();
            services.AddGrpcSwagger();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "gRPC Payment Microservice", Version = "v1" }));

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
