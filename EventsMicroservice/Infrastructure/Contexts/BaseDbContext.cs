using Domain.Entities;
using Domain.Entities.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Infrastructure.Contexts
{
    public abstract class BaseDbContext
    {
        private static bool _isMappingRegistered = false;

        private Dictionary<Type, object> _collections;
        private IMongoDatabase _database;

        protected BaseDbContext(IMongoDatabase database)
        {
            _database = database;
            _collections = new Dictionary<Type, object>();

            RegisterMappings();
        }

        public IMongoDatabase Database => _database;

        public IMongoClient Client => _database.Client;

        public IMongoCollection<T> Collection<T>()
            where T : class, IEntity
        {
            var type = typeof(T);
            if (_collections.ContainsKey(type))
                return (IMongoCollection<T>)_collections[type];

            var collectionName = GetCollectionName<T>();
            var collection = _database.GetCollection<T>(collectionName);
            _collections[type] = collection;

            return collection;
        }

        public string GetCollectionName<T>()
            where T : class, IEntity
        {
            return typeof(T).Name.ToLower();
        }

        private void RegisterMappings()
        {
            if (_isMappingRegistered) return;
            _isMappingRegistered = true;

            // Указываю, что мой Guid Id - действительный идентификатор
            // и что нужно его использовать, а не стандартный ObjectId
            // без изменения сущностей. Т.е. не придётся менять сущности
            // и добавлять для каждой атрибут [BsonId]

            RegisterGuid<Event>();
            RegisterGuid<Seat>();
            RegisterGuid<Review>();
        }

        private void RegisterGuid<T>() 
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
