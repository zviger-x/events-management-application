using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Extensions
{
    public static class MongoCollectionExtensions
    {
        // InsertOneAsync
        public static Task InsertOneWithSessionAsync<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            T document,
            InsertOneOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return session != null
                ? collection.InsertOneAsync(session, document, options, cancellationToken)
                : collection.InsertOneAsync(document, options, cancellationToken);
        }

        // InsertManyAsync
        public static Task InsertManyWithSessionAsync<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            IEnumerable<T> documents,
            InsertManyOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return session != null
                ? collection.InsertManyAsync(session, documents, options, cancellationToken)
                : collection.InsertManyAsync(documents, options, cancellationToken);
        }

        // UpdateOneAsync
        public static Task<UpdateResult> UpdateOneWithSessionAsync<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            UpdateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return session != null
                ? collection.UpdateOneAsync(session, filter, update, options, cancellationToken)
                : collection.UpdateOneAsync(filter, update, options, cancellationToken);
        }

        // UpdateManyAsync
        public static Task<UpdateResult> UpdateManyWithSessionAsync<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            UpdateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return session != null
                ? collection.UpdateManyAsync(session, filter, update, options, cancellationToken)
                : collection.UpdateManyAsync(filter, update, options, cancellationToken);
        }

        // ReplaceOneAsync
        public static Task<ReplaceOneResult> ReplaceOneWithSessionAsync<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            T replacement,
            ReplaceOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return session != null
                ? collection.ReplaceOneAsync(session, filter, replacement, options, cancellationToken)
                : collection.ReplaceOneAsync(filter, replacement, options, cancellationToken);
        }

        // ReplaceManyAsync
        public static async Task<IEnumerable<ReplaceOneResult>> ReplaceManyWithSessionAsync<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            IEnumerable<(FilterDefinition<T> Filter, T Replacement)> replacements,
            ReplaceOptions options = null,
            CancellationToken cancellationToken = default)
        {
            var tasks = replacements.Select(pair =>
                session != null
                    ? collection.ReplaceOneAsync(session, pair.Filter, pair.Replacement, options, cancellationToken)
                    : collection.ReplaceOneAsync(pair.Filter, pair.Replacement, options, cancellationToken)
            );
            return await Task.WhenAll(tasks);
        }

        // DeleteOneAsync
        public static Task<DeleteResult> DeleteOneWithSessionAsync<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            DeleteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return session != null
                ? collection.DeleteOneAsync(session, filter, options, cancellationToken)
                : collection.DeleteOneAsync(filter, options, cancellationToken);
        }

        // DeleteManyAsync
        public static Task<DeleteResult> DeleteManyWithSessionAsync<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            DeleteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return session != null
                ? collection.DeleteManyAsync(session, filter, options, cancellationToken)
                : collection.DeleteManyAsync(filter, options, cancellationToken);
        }

        // Find with mongo filter
        public static IFindFluent<T, T> FindWithSession<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            FindOptions options = null)
        {
            return session != null
                ? collection.Find(session, filter, options)
                : collection.Find(filter, options);
        }


        // Find with expression filter
        public static IFindFluent<T, T> FindWithSession<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            Expression<Func<T, bool>> filter,
            FindOptions options = null)
        {
            return session != null
                ? collection.Find(session, filter, options)
                : collection.Find(filter, options);
        }

        // FindAsync
        public static Task<IAsyncCursor<T>> FindWithSessionAsync<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            FindOptions<T, T> options = null,
            CancellationToken cancellationToken = default)
        {
            return session != null
                ? collection.FindAsync(session, filter, options, cancellationToken)
                : collection.FindAsync(filter, options, cancellationToken);
        }
        
        // Aggregate
        public static IAggregateFluent<T> AggregateWithSession<T>(
            this IMongoCollection<T> collection,
            IClientSessionHandle session,
            AggregateOptions options = null)
        {
            return session != null
                ? collection.Aggregate(session, options)
                : collection.Aggregate(options);
        }
    }
}
