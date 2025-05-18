using MediatR;
using PaymentAPI.Extensions;
using PaymentAPI.Services;
using Serilog;
using Shared.Configuration;
using Shared.Extensions;
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

            // Add configs
            configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("/app/config/grpc-connections.json", optional: true)
                .AddJsonFile("/app/config/kafka-server-settings.json", optional: true)
                .AddEnvironmentVariables();
            var kafkaServerConfig = services.ConfigureAndReceive<KafkaServerConfig>(configuration, "KafkaServerConfig");
            var grpcConnections = services.ConfigureAndReceive<GrpcConnectionsConfig>(configuration, "GrpcConnections");

            // Add logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: CustomConsoleThemes.SixteenEnhanced)
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();
            logging.ClearProviders();
            logging.AddSerilog();

            services.AddAutoMapper();

            // BLL
            services.AddClients(grpcConnections);
            services.AddValidators();
            services.AddSagas();
            services.AddMediatR();
            services.AddKafkaProducers();

            // API
            services.AddGrpcWithInterceptors();

            var app = builder.Build();

            app.MapGrpcService<PaymentService>();

            app.Run();
        }
    }
}
