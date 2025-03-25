using Domain.Entities;
using Domain.Entities.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Application.Contexts
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

        protected string GetCollectionName<T>()
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
            // и добавлять каждой атрибут [BsonId]

            #region -- Guid as ID --
            BsonClassMap.RegisterClassMap<Event>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            });

            BsonClassMap.RegisterClassMap<Seat>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            });

            BsonClassMap.RegisterClassMap<Review>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
            });
            #endregion
        }
    }
}
