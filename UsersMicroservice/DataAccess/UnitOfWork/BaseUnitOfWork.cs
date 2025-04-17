using DataAccess.Contexts;
using DataAccess.Entities.Interfaces;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.UnitOfWork
{
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
            _repositories = new();
        }

        public IRepository<T> Repository<T>()
            where T : class, IEntity
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = _serviceProvider.GetRequiredService<IRepository<T>>();
                _repositories[type] = repositoryInstance;
            }

            return (IRepository<T>)_repositories[type];
        }

        public virtual async Task BeginTransactionAsync(CancellationToken token = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(token);
        }

        public virtual async Task CommitTransactionAsync(CancellationToken token = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _context.SaveChangesAsync(token);
            await _transaction.CommitAsync(token);
            _transaction.Dispose();
            _transaction = null;
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken token = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _transaction.RollbackAsync(token);
            _transaction.Dispose();
            _transaction = null;
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
