using Infrastructure.Contexts;
using Infrastructure.Extensions;
using Infrastructure.Mongo.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Mongo
{
    internal class MongoCollectionWrapper<TDocument> : IMongoCollectionWrapper<TDocument>
    {
        protected readonly IMongoCollection<TDocument> _collection;
        protected readonly TransactionContext _transactionContext;

        public MongoCollectionWrapper(IMongoCollection<TDocument> collection, TransactionContext transactionContext)
        {
            _collection = collection;
            _transactionContext = transactionContext;
        }

        protected IClientSessionHandle CurrentSession { get => _transactionContext.Session; }

        public Task InsertOneAsync(TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default)
        {
            return InsertOneAsync(CurrentSession, document, options, cancellationToken);
        }

        public Task InsertOneAsync(IClientSessionHandle session, TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default)
        {
            return _collection.InsertOneWithSessionAsync(session, document, options, cancellationToken);
        }

        public Task InsertManyAsync(IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default)
        {
            return InsertManyAsync(CurrentSession, documents, options, cancellationToken);
        }

        public Task InsertManyAsync(IClientSessionHandle session, IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default)
        {
            return _collection.InsertManyWithSessionAsync(session, documents, options, cancellationToken);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default)
        {
            return ReplaceOneAsync(CurrentSession, filter, replacement, options, cancellationToken);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default)
        {
            return _collection.ReplaceOneWithSessionAsync(session, filter, replacement, options, cancellationToken);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TDocument, bool>> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default)
        {
            return ReplaceOneAsync(CurrentSession, filter, replacement, options, cancellationToken);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default)
        {
            return _collection.ReplaceOneWithSessionAsync(session, filter, replacement, options, cancellationToken);
        }

        public Task<BulkWriteResult<TDocument>> ReplaceManyAsync(IEnumerable<ReplaceOneModel<TDocument>> replacements, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
        {
            return ReplaceManyAsync(CurrentSession, replacements, options, cancellationToken);
        }

        public Task<BulkWriteResult<TDocument>> ReplaceManyAsync(IClientSessionHandle session, IEnumerable<ReplaceOneModel<TDocument>> replacements, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
        {
            return _collection.ReplaceManyWithSessionAsync(session, replacements, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
        {
            return DeleteOneAsync(CurrentSession, filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteOneWithSessionAsync(session, filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteOneAsync(Expression<Func<TDocument, bool>> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
        {
            return DeleteOneAsync(CurrentSession, filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteOneAsync(IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteOneWithSessionAsync(session, filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteManyAsync(FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
        {
            return DeleteManyAsync(CurrentSession, filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteManyAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteManyWithSessionAsync(session, filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteManyAsync(Expression<Func<TDocument, bool>> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
        {
            return DeleteManyAsync(CurrentSession, filter, options, cancellationToken);
        }

        public Task<DeleteResult> DeleteManyAsync(IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteManyWithSessionAsync(session, filter, options, cancellationToken);
        }

        public IAggregateFluent<TDocument> Aggregate(AggregateOptions options = null)
        {
            return Aggregate(CurrentSession, options);
        }

        public IAggregateFluent<TDocument> Aggregate(IClientSessionHandle session, AggregateOptions options = null)
        {
            return _collection.AggregateWithSession(session, options);
        }

        public IFindFluent<TDocument, TDocument> Find(FilterDefinition<TDocument> filter, FindOptions options = null)
        {
            return Find(CurrentSession, filter, options);
        }

        public IFindFluent<TDocument, TDocument> Find(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions options = null)
        {
            return _collection.FindWithSession(session, filter, options);
        }

        public IFindFluent<TDocument, TDocument> Find(Expression<Func<TDocument, bool>> filter, FindOptions options = null)
        {
            return Find(CurrentSession, filter, options);
        }

        public IFindFluent<TDocument, TDocument> Find(IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, FindOptions options = null)
        {
            return _collection.FindWithSession(session, filter, options);
        }

        public Task<IAsyncCursor<TDocument>> FindAsync(FilterDefinition<TDocument> filter, FindOptions<TDocument, TDocument> options = null, CancellationToken cancellationToken = default)
        {
            return FindAsync(CurrentSession, filter, options, cancellationToken);
        }

        public Task<IAsyncCursor<TDocument>> FindAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TDocument> options = null, CancellationToken cancellationToken = default)
        {
            return _collection.FindWithSessionAsync(session, filter, options, cancellationToken);
        }

        public Task<IAsyncCursor<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, FindOptions<TDocument, TDocument> options = null, CancellationToken cancellationToken = default)
        {
            return FindAsync(CurrentSession, filter, options, cancellationToken);
        }

        public Task<IAsyncCursor<TDocument>> FindAsync(IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, FindOptions<TDocument, TDocument> options = null, CancellationToken cancellationToken = default)
        {
            return _collection.FindWithSessionAsync(session, filter, options, cancellationToken);
        }
    }
}
