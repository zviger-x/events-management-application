using DataAccess.Contexts;
using DataAccess.Entities.Interfaces;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories
{
    internal abstract class BaseRepository<T> : IRepository<T>
        where T : IEntity
    {
        protected BaseRepository(UserDbContext context)
        {
            _context = context;
        }

        protected UserDbContext _context { get; set; }

        public virtual async Task CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
        }

        public virtual Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity)
        {
            _context.Remove(entity);
            return Task.CompletedTask;
        }

        public abstract Task<T> GetByIdAsync(int id);

        public abstract IQueryable<T> GetAll();

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
