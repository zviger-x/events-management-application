using Domain.Entities.Interfaces;
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
    }
}
