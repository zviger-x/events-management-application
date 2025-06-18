using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Mongo.Interfaces
{
    /// <summary>
    /// Provides a safe abstraction over IMongoCollection to ensure operations are executed within a given transaction context.
    /// Automatically applies the session from TransactionContext to all operations.
    /// </summary>
    public interface IMongoCollectionWrapper<TDocument>
    {
        /// <summary>
        /// Inserts a single document into the collection.
        /// </summary>
        Task InsertOneAsync(TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts a single document with a custom session.
        /// </summary>
        Task InsertOneAsync(IClientSessionHandle session, TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts multiple documents into the collection.
        /// </summary>
        Task InsertManyAsync(IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts multiple documents with a custom session.
        /// </summary>
        Task InsertManyAsync(IClientSessionHandle session, IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces a single document matching the filter.
        /// </summary>
        Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces a single document matching the filter.
        /// </summary>
        Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TDocument, bool>> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces a single document matching the filter using a custom session.
        /// </summary>
        Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces a single document matching the filter using a custom session.
        /// </summary>
        Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, TDocument replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces multiple documents using bulk write.
        /// </summary>
        Task<BulkWriteResult<TDocument>> ReplaceManyAsync(IEnumerable<ReplaceOneModel<TDocument>> replacements, BulkWriteOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces multiple documents using bulk write with a custom session.
        /// </summary>
        Task<BulkWriteResult<TDocument>> ReplaceManyAsync(IClientSessionHandle session, IEnumerable<ReplaceOneModel<TDocument>> replacements, BulkWriteOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a single document matching the filter.
        /// </summary>
        Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a single document matching the filter.
        /// </summary>
        Task<DeleteResult> DeleteOneAsync(Expression<Func<TDocument, bool>> filter, DeleteOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a single document with a custom session.
        /// </summary>
        Task<DeleteResult> DeleteOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a single document with a custom session.
        /// </summary>
        Task<DeleteResult> DeleteOneAsync(IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, DeleteOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple documents matching the filter.
        /// </summary>
        Task<DeleteResult> DeleteManyAsync(FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple documents matching the filter.
        /// </summary>
        Task<DeleteResult> DeleteManyAsync(Expression<Func<TDocument, bool>> filter, DeleteOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple documents with a custom session.
        /// </summary>
        Task<DeleteResult> DeleteManyAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple documents with a custom session.
        /// </summary>
        Task<DeleteResult> DeleteManyAsync(IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, DeleteOptions options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a Find operation returning an IFindFluent for further query composition.
        /// </summary>
        IFindFluent<TDocument, TDocument> Find(FilterDefinition<TDocument> filter, FindOptions options = null);

        /// <summary>
        /// Executes a Find operation with an expression filter.
        /// </summary>
        IFindFluent<TDocument, TDocument> Find(Expression<Func<TDocument, bool>> filter, FindOptions options = null);

        /// <summary>
        /// Executes a Find operation returning an IFindFluent for further query composition, using a custom session.
        /// </summary>
        IFindFluent<TDocument, TDocument> Find(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions options = null);

        /// <summary>
        /// Executes a Find operation with an expression filter, using a custom session.
        /// </summary>
        IFindFluent<TDocument, TDocument> Find(IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, FindOptions options = null);

        /// <summary>
        /// Executes a Find operation asynchronously.
        /// </summary>
        Task<IAsyncCursor<TDocument>> FindAsync(FilterDefinition<TDocument> filter, FindOptions<TDocument, TDocument> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a Find operation asynchronously.
        /// </summary>
        Task<IAsyncCursor<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, FindOptions<TDocument, TDocument> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a Find operation asynchronously with a custom session.
        /// </summary>
        Task<IAsyncCursor<TDocument>> FindAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TDocument> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a Find operation asynchronously with a custom session.
        /// </summary>
        Task<IAsyncCursor<TDocument>> FindAsync(IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, FindOptions<TDocument, TDocument> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes an aggregation pipeline.
        /// </summary>
        IAggregateFluent<TDocument> Aggregate(AggregateOptions options = null);

        /// <summary>
        /// Executes an aggregation pipeline with a custom session.
        /// </summary>
        IAggregateFluent<TDocument> Aggregate(IClientSessionHandle session, AggregateOptions options = null);
    }
}
