using Infrastructure.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Shared.Entities.Interfaces;
using Shared.Repositories.Interfaces;
using Shared.UnitOfWork;

namespace Infrastructure.UnitOfWork
{
    public abstract class BaseUnitOfWork : IBaseUnitOfWork
    {
        protected readonly EventDbContext _context;
        protected readonly TransactionContext _transactionContext;
        protected readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<Type, object> _repositories;

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
            _transactionContext.Session = await _context.Client.StartSessionAsync(null, cancellationToken: token);
            _transactionContext.Session.StartTransaction();
        }

        public virtual async Task CommitTransactionAsync(CancellationToken token = default)
        {
            if (_transactionContext.Session == null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _transactionContext.Session.CommitTransactionAsync(token);
            _transactionContext.Session.Dispose();
            _transactionContext.Session = null;
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken token = default)
        {
            if (_transactionContext.Session == null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _transactionContext.Session.AbortTransactionAsync(token);
            _transactionContext.Session.Dispose();
            _transactionContext.Session = null;
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

        public async Task<T> InvokeWithTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken token = default)
        {
            await BeginTransactionAsync(token);
            try
            {
                var result = await action(token);
                await CommitTransactionAsync(token);

                return result;
            }
            catch
            {
                await RollbackTransactionAsync(token);
                throw;
            }
        }

        public virtual void Dispose()
        {
            _transactionContext.Session?.Dispose();
            _transactionContext.Session = null;
        }
    }
}
