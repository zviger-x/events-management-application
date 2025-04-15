using MongoDB.Bson.Serialization;
using Serilog;
using Shared.Entities.Interfaces;
using System.Reflection;

namespace Infrastructure.Mongo
{
    public static class MongoGuidConventionRegistrar
    {
        /// <summary>
        /// Registers Id as BsonId for entities of type <see cref="IEntity"/>
        /// </summary>
        /// <param name="assemblies">Assemblies in which entities that inherit IEntity will be searched</param>
        public static void Register(Assembly[] assemblies)
        {
            // Указываю, что мой Guid Id - действительный идентификатор
            // и что нужно его использовать, а не стандартный ObjectId
            // без изменения сущностей. Т.е. не придётся менять сущности
            // и добавлять для каждой атрибут [BsonId]

            Log.Information($"Registering Guid for MongoDB...");

            var entityTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();

            var method = typeof(MongoGuidConventionRegistrar).GetMethod(nameof(RegisterGuid), BindingFlags.NonPublic | BindingFlags.Static);
            var maxLength = entityTypes.Max(t => t.Name.Length);
            foreach (var type in entityTypes)
            {
                Log.Information($"Register Guid as default mongo Id for {{{type.Name}}}{new(' ', maxLength - type.Name.Length)} from [{type.Assembly.GetName().Name}]");

                var genericMethod = method.MakeGenericMethod(type);
                genericMethod.Invoke(null, null);
            }

            Log.Information($"Registration completed");
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
