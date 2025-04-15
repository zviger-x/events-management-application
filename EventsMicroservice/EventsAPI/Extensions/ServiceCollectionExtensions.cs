using Application.Repositories.Interfaces;
using Application.Validation.Validators.Interfaces;
using Domain.Entities;
using EventsAPI.Configuration;
using Infrastructure.Repositories;
using Infrastructure.Validation.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Serilog;
using Shared.Caching;
using Shared.Caching.Interfaces;
using Shared.Entities.Interfaces;
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

            // Указываю, что мой Guid Id - действительный идентификатор
            // и что нужно его использовать, а не стандартный ObjectId
            // без изменения сущностей. Т.е. не придётся менять сущности
            // и добавлять для каждой атрибут [BsonId]

            // TODO: Вынести эту логику куда-нибудь в другое место
            Log.Information($"Registering Guid for MongoDB...");

            var entityTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Union([Assembly.GetAssembly(typeof(Event))])
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();

            var method = typeof(ServiceCollectionExtensions).GetMethod(nameof(RegisterGuid), BindingFlags.NonPublic | BindingFlags.Static);
            var maxLength = entityTypes.Max(t => t.Name.Length);
            foreach (var type in entityTypes)
            {
                Log.Information($"Register Guid as default mongo Id for {{{type.Name}}}{new(' ', maxLength - type.Name.Length)} from [{type.Assembly.GetName().Name}]");

                var genericMethod = method.MakeGenericMethod(type);
                genericMethod.Invoke(null, null);
            }

            Log.Information($"Registration completed");
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

        private static void RegisterGuid<T>()
            where T : class, IEntity
        {
            BsonClassMap.RegisterClassMap<T>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            });
        }
    }
}
