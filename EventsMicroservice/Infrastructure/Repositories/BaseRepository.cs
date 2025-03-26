using Application.Repositories.Interfaces;
using Domain.Entities.Interfaces;
using Domain.Entities;
using MongoDB.Driver;
using Infrastructure.Contexts;

#pragma warning disable CS8603
namespace Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T>
        where T : class, IEntity
    {
        protected BaseRepository(EventDbContext context)
        {
            _context = context;
        }

        protected EventDbContext _context { get; private set; }

        public virtual async Task CreateAsync(T entity, CancellationToken token = default)
        {
            await _context.Collection<T>().InsertOneAsync(entity, cancellationToken: token);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken token = default)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            await _context.Collection<T>().ReplaceOneAsync(filter, entity, cancellationToken: token);
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
        ///     if (deleteResult.DeletedCount > 0)
        ///         // Deleting related entities
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

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            using var cursor = await _context.Collection<T>().FindAsync(filter, cancellationToken: token);
            return await cursor.FirstOrDefaultAsync(token);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
        {
            return await _context.Collection<T>().Find(_ => true).ToListAsync();
        }

        public virtual async Task<PagedCollection<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            var query = _context.Collection<T>().Find(_ => true);

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
