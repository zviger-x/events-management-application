using DataAccess.Contexts;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Entities.Interfaces;
using Shared.Repositories.Interfaces;

namespace DataAccess.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T>
        where T : class, IEntity
    {
        protected BaseRepository(UserDbContext context)
        {
            _context = context;
        }

        protected UserDbContext _context { get; set; }

        public virtual async Task<Guid> CreateAsync(T entity, CancellationToken token = default)
        {
            await _context.AddAsync(entity, token);
            await _context.SaveChangesAsync(token);

            _context.Entry(entity).State = EntityState.Detached;

            return entity.Id;
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken token = default)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync(token);

            _context.Entry(entity).State = EntityState.Detached;
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken token = default)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(token);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var entity = await _context.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, token);

            return entity;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync(token);
        }

        public virtual async Task<PagedCollection<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            var query = _context.Set<T>().AsNoTracking();

            var totalCount = await query.CountAsync(token);
            var totalPages = (int)Math.Ceiling(totalCount / (float)pageSize);

            var page = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);

            return new PagedCollection<T>
            {
                Items = page,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }

        public virtual async Task CreateManyAsync(IEnumerable<T> entities, CancellationToken token = default)
        {
            await _context.BulkInsertAsync(entities, cancellationToken: token);
        }

        public virtual async Task UpdateManyAsync(IEnumerable<T> entities, CancellationToken token = default)
        {
            await _context.BulkUpdateAsync(entities, cancellationToken: token);
        }

        public virtual async Task DeleteManyAsync(IEnumerable<T> entities, CancellationToken token = default)
        {
            await _context.BulkDeleteAsync(entities, cancellationToken: token);
        }

        public virtual void Dispose()
        {
            _context.Dispose();
        }
    }
}
