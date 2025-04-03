using Application.Repositories.Interfaces;
using Application.UnitOfWork.Interfaces;
using Domain.Entities.Interfaces;
using Infrastructure.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitOfWork
{
    public abstract class BaseUnitOfWork : IBaseUnitOfWork
    {
        protected readonly EventDbContext _context;
        protected readonly TransactionContext _transactionContext;
        protected readonly IServiceProvider _serviceProvider;

        private Dictionary<Type, object> _repositories;

        public BaseUnitOfWork(EventDbContext context, TransactionContext transactionContext, IServiceProvider serviceProvider)
        {
            _context = context;
            _transactionContext = transactionContext;
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
            _transactionContext.CurrentSession = await _context.Client.StartSessionAsync(null, cancellationToken: token);
            _transactionContext.CurrentSession.StartTransaction();
        }

        public virtual async Task CommitTransactionAsync(CancellationToken token = default)
        {
            if (_transactionContext.CurrentSession == null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _transactionContext.CurrentSession.CommitTransactionAsync(token);
            _transactionContext.CurrentSession.Dispose();
            _transactionContext.CurrentSession = null;
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken token = default)
        {
            if (_transactionContext.CurrentSession == null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _transactionContext.CurrentSession.AbortTransactionAsync();
            _transactionContext.CurrentSession.Dispose();
            _transactionContext.CurrentSession = null;
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
            _transactionContext.CurrentSession?.Dispose();
            _transactionContext.CurrentSession = null;
        }
    }
}
