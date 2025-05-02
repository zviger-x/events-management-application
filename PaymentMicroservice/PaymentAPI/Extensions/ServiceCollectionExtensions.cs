using Application.Clients;
using Application.MediatR.Behaviours;
using FluentValidation;
using Infrastructure.Clients;
using MediatR;
using Shared.Grpc.Interceptors;
using System.Reflection;

namespace PaymentAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper([
                Assembly.Load("Application"),
                Assembly.Load("Infrastructure"),
                Assembly.Load("PaymentAPI")]);
        }

        public static void AddClients(this IServiceCollection services)
        {
            services.AddScoped<IUserClient, UserClientStub>();
            services.AddScoped<IPaymentClient, PaymentClientStub>();
        }

        public static void AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.Load("Infrastructure"));
        }

        public static void AddMediatR(this IServiceCollection services)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        public static void AddGrpcWithInterceptors(this IServiceCollection services)
        {
            services.AddGrpc(o =>
            {
                o.Interceptors.Add<GrpcExceptionInterceptor>();
            }).AddJsonTranscoding();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "gRPC Payment Microservice", Version = "v1" }));
        }
    }
}
