using Application.Repositories.Interfaces;
using Application.UseCases.Interfaces;
using Application.Validation.Validators.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
using Infrastructure.UseCases.EventUseCases;
using Infrastructure.Validation.Validators;

namespace EventsAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Event>, EventRepository>();
            services.AddScoped<IEventRepository, EventRepository>();

            services.AddScoped<IRepository<Seat>, SeatRepository>();
            services.AddScoped<ISeatRepository, SeatRepository>();

            services.AddScoped<IRepository<Review>, ReviewRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
        }

        public static void AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IEventValidator, EventValidator>();
            services.AddScoped<ISeatValidator, SeatValidator>();
            services.AddScoped<IReviewValidator, ReviewValidator>();
        }

        public static void AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<ICreateUseCaseAsync<Event>, EventCreateUseCase>();
            services.AddScoped<IUpdateUseCaseAsync<Event>, EventUpdateUseCase>();
            services.AddScoped<IDeleteUseCaseAsync<Event>, EventDeleteUseCase>();
            services.AddScoped<IGetByIdUseCaseAsync<Event>, EventGetByIdUseCase>();
            services.AddScoped<IGetAllUseCaseAsync<Event>, EventGetAllUseCase>();
            services.AddScoped<IGetPagedUseCaseAsync<Event>, EventGetPagedUseCase>();
        }

        // public static void AddCachingServices(this IServiceCollection services)
        // {
        // }
    }
}
