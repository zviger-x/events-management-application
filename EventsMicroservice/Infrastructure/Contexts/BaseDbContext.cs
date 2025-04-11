using Infrastructure.Mongo.Interfaces;
using Infrastructure.Mongo;
using MongoDB.Driver;
using Shared.Entities.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Contexts
{
    public abstract class BaseDbContext
    {
        private readonly Dictionary<Type, object> _collections = new();
        private readonly IMongoDatabase _database;
        private readonly IServiceProvider _serviceProvider;

        protected BaseDbContext(IMongoDatabase database, IServiceProvider serviceProvider)
        {
            _database = database;
            _serviceProvider = serviceProvider;
        }

        public IMongoDatabase Database => _database;

        public IMongoClient Client => _database.Client;

        public IMongoCollectionWrapper<T> Collection<T>()
            where T : class, IEntity
        {
            var type = typeof(T);
            if (_collections.ContainsKey(type))
                return (IMongoCollectionWrapper<T>)_collections[type];

            var collectionName = GetCollectionName<T>();
            var collection = _database.GetCollection<T>(collectionName);

            var transactionContext = _serviceProvider.GetRequiredService<TransactionContext>();
            var wrapper = new MongoCollectionWrapper<T>(collection, transactionContext);

            _collections[type] = wrapper;
            return wrapper;
        }

        public string GetCollectionName<T>()
            where T : class, IEntity
        {
            return typeof(T).Name.ToLower();
        }
    }
}
