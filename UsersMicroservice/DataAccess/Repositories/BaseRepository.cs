using DataAccess.Contexts;
using DataAccess.Entities;
using DataAccess.Entities.Interfaces;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8603
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

        public virtual async Task CreateAsync(T entity, CancellationToken token = default)
        {
            await _context.AddAsync(entity, token);
        }

        public virtual Task UpdateAsync(T entity, CancellationToken token = default)
        {
            _context.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity, CancellationToken token = default)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.Set<T>().FindAsync([id], cancellationToken: token);
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

        public virtual async Task SaveChangesAsync(CancellationToken token = default)
        {
            await _context.SaveChangesAsync(token);
        }

        public virtual void Dispose()
        {
            _context.Dispose();
        }
    }
}
