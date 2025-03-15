using DataAccess.Contexts;
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

        public virtual IEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking().AsEnumerable();
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
