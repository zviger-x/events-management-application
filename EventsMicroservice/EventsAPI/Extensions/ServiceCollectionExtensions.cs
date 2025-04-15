using Application.Repositories.Interfaces;
using Application.Validation.Validators.Interfaces;
using Domain.Entities;
using EventsAPI.Configuration;
using Infrastructure.Mongo;
using Infrastructure.Repositories;
using Infrastructure.Validation.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Shared.Caching;
using Shared.Caching.Interfaces;
using Shared.Repositories.Interfaces;
using Shared.Validation.Interfaces;
using Shared.Validation.Validators;
using StackExchange.Redis;
using System.Reflection;

namespace EventsAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMongoServer(this IServiceCollection services, MongoServerConfig config)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            services.AddSingleton<IMongoClient>(new MongoClient(config.ConnectionString));
            services.AddScoped<IMongoDatabase>(provider =>
            {
                var client = provider.GetRequiredService<IMongoClient>();
                return client.GetDatabase(config.DatabaseName);
            });

            // Register Id for all entities as BsonId for mongo
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Union([Assembly.GetAssembly(typeof(Event))])
                .ToArray();
            MongoGuidConventionRegistrar.Register(assemblies);
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Event>, EventRepository>();
            services.AddScoped<IEventRepository, EventRepository>();

            services.AddScoped<IRepository<EventComment>, EventCommentRepository>();
            services.AddScoped<IEventCommentRepository, EventCommentRepository>();

            services.AddScoped<IRepository<Seat>, SeatRepository>();
            services.AddScoped<ISeatRepository, SeatRepository>();

            services.AddScoped<IRepository<SeatConfiguration>, SeatConfigurationRepository>();
            services.AddScoped<ISeatConfigurationRepository, SeatConfigurationRepository>();

            services.AddScoped<IRepository<EventUser>, EventUserRepository>();
            services.AddScoped<IEventUserRepository, EventUserRepository>();
        }

        public static void AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IEventValidator, EventValidator>();
            services.AddScoped<ICreateEventDtoValidator, CreateEventDtoValidator>();
            services.AddScoped<IUpdateEventDtoValidator, UpdateEventDtoValidator>();

            services.AddScoped<IEventUserValidator, EventUserValidator>();
            services.AddScoped<ICreateEventUserDtoValidator, CreateEventUserDtoValidator>();

            services.AddScoped<IEventCommentValidator, EventCommentValidator>();
            services.AddScoped<ICreateEventCommentDtoValidator, CreateEventCommentDtoValidator>();
            services.AddScoped<IUpdateEventCommentDtoValidator, UpdateEventCommentDtoValidator>();

            services.AddScoped<ISeatValidator, SeatValidator>();

            services.AddScoped<ISeatConfigurationValidator, SeatConfigurationValidator>();
            services.AddScoped<ICreateSeatConfigurationDtoValidator, CreateSeatConfigurationDtoValidator>();
            services.AddScoped<IUpdateSeatConfigurationDtoValidator, UpdateSeatConfigurationDtoValidator>();

            // From shared
            services.AddScoped<IPageParametersValidator, PageParametersValidator>();
        }

        public static void AddRedisServer(this IServiceCollection services, RedisServerConfig config)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(config.ConnectionString));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.ConnectionString;
                options.InstanceName = config.CachePrefix;
            });
        }

        public static void AddCachingServices(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
        }
    }
}
