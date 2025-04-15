using Infrastructure.Contexts;
using MongoDB.Driver;
using Shared.Common;
using Shared.Entities.Interfaces;
using Shared.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
#warning TODO: Убрать обнуление ID из репозитория, это не его ответственность.
    public abstract class BaseRepository<T> : IRepository<T>
        where T : class, IEntity
    {

        protected readonly EventDbContext _context;

        protected BaseRepository(EventDbContext context)
        {
            _context = context;
        }

        public virtual async Task<Guid> CreateAsync(T entity, CancellationToken token = default)
        {
            entity.Id = Guid.NewGuid();
            await _context.Collection<T>().InsertOneAsync(entity, cancellationToken: token);

            return entity.Id;
        }

        public virtual async Task CreateManyAsync(IEnumerable<T> entities, CancellationToken token = default)
        {
            foreach (var entity in entities)
                entity.Id = Guid.NewGuid();

            await _context.Collection<T>().InsertManyAsync(entities, cancellationToken: token);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken token = default)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            await _context.Collection<T>().ReplaceOneAsync(filter, entity, cancellationToken: token);
        }

        public virtual async Task UpdateManyAsync(IEnumerable<T> entities, CancellationToken token = default)
        {
            var requests = entities.Select(entity =>
                new ReplaceOneModel<T>(
                    Builders<T>.Filter.Eq(e => e.Id, entity.Id),
                    entity));

            await _context.Collection<T>().ReplaceManyAsync(requests, cancellationToken: token);
        }

        /// <remarks>
        /// Removes an entity and automatically saves the changes to the database.
        /// Does not support cascading deletes.
        /// <para>Example of using cascade delete:</para>
        /// <code>
        /// using var session = await _context.Client.StartSessionAsync(cancellationToken: token);
        /// session.StartTransaction();
        /// try
        /// {
        ///     var deleteResult = await _context.[Collection].DeleteOneAsync(session, e => e.Id == entity.Id, cancellationToken: token);
        ///     
        ///     // Deleting related entities
        ///     if (deleteResult.DeletedCount > 0)
        ///         await _context.[RelatedCollection].DeleteManyAsync(session, re => re.eId == entity.Id, cancellationToken: token);
        ///         
        ///     await session.CommitTransactionAsync(token);
        /// }
        /// catch
        /// {
        ///     await session.AbortTransactionAsync(token);
        ///     throw;
        /// }
        /// </code>
        /// </remarks>
        public virtual async Task DeleteAsync(T entity, CancellationToken token = default)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            await _context.Collection<T>().DeleteOneAsync(filter, cancellationToken: token);
        }

        /// <remarks>
        /// Removes multiple entities and automatically saves the changes to the database within a transaction.
        /// Does not support cascading deletes by default.
        /// </remarks>
        public virtual async Task DeleteManyAsync(IEnumerable<T> entity, CancellationToken token = default)
        {
            var ids = entity.Select(e => e.Id);
            var filter = Builders<T>.Filter.In(e => e.Id, ids);
            await _context.Collection<T>().DeleteManyAsync(filter, cancellationToken: token);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            using var cursor = await _context.Collection<T>().FindAsync(filter, cancellationToken: token);
            return await cursor.FirstOrDefaultAsync(token);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
        {
            return await _context.Collection<T>().Find(_ => true).ToListAsync(token);
        }

        public virtual async Task<PagedCollection<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            return await GetPagedByFilterAsync(_ => true, pageNumber, pageSize, token);
        }

        protected async Task<PagedCollection<T>> GetPagedByFilterAsync(Expression<Func<T, bool>> filterExpression, int pageNumber, int pageSize, CancellationToken token = default)
        {
            var query = _context.Collection<T>().Find(filterExpression);

            var totalCount = await query.CountDocumentsAsync(token);
            var totalPages = (int)Math.Ceiling(totalCount / (float)pageSize);

            var page = await query
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(token);

            return new PagedCollection<T>
            {
                Items = page,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }

        public virtual void Dispose()
        {
        }
    }
}
