using MongoDB.Bson.Serialization;
using Shared.Entities.Interfaces;
using System.Reflection;
using static Shared.Logging.Extensions.SerilogExtensions;

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

            Log.Information("Registering Guid for MongoDB...");

            var entityTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();

            var method = typeof(MongoGuidConventionRegistrar).GetMethod(nameof(RegisterGuid), BindingFlags.NonPublic | BindingFlags.Static);
            var maxLength = entityTypes.Max(t => t.Name.Length);
            foreach (var type in entityTypes)
            {
                var typeName = type.Name;
                var offset = new string(' ', maxLength - type.Name.Length);
                var typeAssemblyName = type.Assembly.GetName().Name;
                Log.InformationInterpolated($"Register Guid as default mongo Id for {{{typeName}}}{offset} from [{typeAssemblyName}]");

                var genericMethod = method.MakeGenericMethod(type);
                genericMethod.Invoke(null, null);
            }

            Log.Information("Registration completed");
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
