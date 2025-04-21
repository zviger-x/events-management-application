using Application.MediatR.Behaviours;
using Application.Repositories.Interfaces;
using Domain.Entities;
using EventsAPI.Configuration;
using FluentValidation;
using Infrastructure.Mongo;
using Infrastructure.Repositories;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Shared.Caching.Services;
using Shared.Caching.Services.Interfaces;
using Shared.Repositories.Interfaces;
using StackExchange.Redis;
using System.Reflection;
using static Shared.Logging.Extensions.SerilogExtensions;

namespace EventsAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMongoServer(this IServiceCollection services, MongoServerConfig config)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            Log.Information("Trying to connected to MongoDB Server...");
            var client = new MongoClient(config.ConnectionString);
            try
            {
                _ = client.ListDatabases();
                Log.Information("Successfully connected to MongoDB");
            }
            catch (Exception ex)
            {
                Log.ErrorInterpolated(ex, $"Error while connecting to MongoDB: {ex.Message}");
            }

            services.AddSingleton<IMongoClient>(client);
            services.AddScoped<IMongoDatabase>(provider =>
            {
                var mongoClient = provider.GetRequiredService<IMongoClient>();
                return mongoClient.GetDatabase(config.DatabaseName);
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
            services.AddValidatorsFromAssembly(Assembly.Load("Infrastructure"));
            services.AddValidatorsFromAssembly(Assembly.Load("Shared"));
        }

        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
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
