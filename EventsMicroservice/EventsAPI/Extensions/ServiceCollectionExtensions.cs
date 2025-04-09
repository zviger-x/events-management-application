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
using Shared.Entities.Interfaces;
using Shared.Repositories.Interfaces;
using System.Reflection;

namespace EventsAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMongoServer(this IServiceCollection services, MongoServerConfig mongoServerConfig)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            
            services.AddSingleton<IMongoClient>(new MongoClient(mongoServerConfig.ConnectionString));
            services.AddScoped<IMongoDatabase>(provider =>
            {
                var client = provider.GetRequiredService<IMongoClient>();
                return client.GetDatabase(mongoServerConfig.DatabaseName);
            });

            // Указываю, что мой Guid Id - действительный идентификатор
            // и что нужно его использовать, а не стандартный ObjectId
            // без изменения сущностей. Т.е. не придётся менять сущности
            // и добавлять для каждой атрибут [BsonId]

            // Вынести эту логику куда-нибудь в другое место
            Log.Information($"Registering Guid for MongoDB...");

            var entityTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();

            var method = typeof(ServiceCollectionExtensions).GetMethod(nameof(RegisterGuid), BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var type in entityTypes)
            {
                Log.Information($"Register Guid as default mongo Id for {{{type.Name}}}");

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
            services.AddScoped<IEventUserValidator, EventUserValidator>();
            services.AddScoped<IEventCommentValidator, EventCommentValidator>();
            services.AddScoped<ISeatValidator, SeatValidator>();
            services.AddScoped<ISeatConfigurationValidator, SeatConfigurationValidator>();

            services.AddScoped<ICreateEventDTOValidator, CreateEventDTOValidator>();
            services.AddScoped<IUpdateEventDTOValidator, UpdateEventDTOValidator>();
        }

        // public static void AddCachingServices(this IServiceCollection services)
        // {
        // }

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
