using DataAccess.Contexts;
using DataAccess.Entities.Interfaces;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.UnitOfWork
{
    #pragma warning disable CS8618
    public abstract class BaseUnitOfWork : IBaseUnitOfWork
    {
        protected readonly UserDbContext _context;
        protected IDbContextTransaction _transaction;
        protected readonly IServiceProvider _serviceProvider;

        private Dictionary<Type, object> _repositories;

        public BaseUnitOfWork(UserDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public IRepository<T> Repository<T>()
            where T : class, IEntity
        {
            if (_repositories == null)
                _repositories = new();

            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = _serviceProvider.GetRequiredService<IRepository<T>>();
                _repositories[type] = repositoryInstance;
            }

            return (IRepository<T>)_repositories[type];
        }

        public virtual async Task SaveChangesAsync(CancellationToken token = default)
        {
            await _context.SaveChangesAsync(token);
        }

        public virtual async Task BeginTransactionAsync(CancellationToken token = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(token);
        }

        public virtual async Task CommitTransactionAsync(CancellationToken token = default)
        {
            await _context.SaveChangesAsync(token);
            await _transaction.CommitAsync(token);
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken token = default)
        {
            await _transaction.RollbackAsync(token);
        }

        public virtual async Task InvokeWithTransactionAsync(Func<CancellationToken, Task> action, CancellationToken token = default)
        {
            await BeginTransactionAsync(token);
            try
            {
                await action(token);
                await CommitTransactionAsync(token);
            }
            catch
            {
                await RollbackTransactionAsync(token);
                throw;
            }
        }

        public virtual void Dispose()
        {
            _context?.Dispose();
            _transaction?.Dispose();
        }
    }
}
