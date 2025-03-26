using Application.Repositories.Interfaces;
using Application.UnitOfWork.Interfaces;
using Domain.Entities.Interfaces;
using Infrastructure.Contexts;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure.UnitOfWork
{
#pragma warning disable CS8618
    public abstract class BaseUnitOfWork : IBaseUnitOfWork
    {
        protected readonly EventDbContext _context;
        protected readonly IServiceProvider _serviceProvider;
        protected IClientSessionHandle? _session;

        private Dictionary<Type, object> _repositories;

        public BaseUnitOfWork(EventDbContext context, IServiceProvider serviceProvider)
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

        public virtual async Task BeginTransactionAsync(CancellationToken token = default)
        {
            _session = await _context.Client.StartSessionAsync(null, cancellationToken: token);
            _session.StartTransaction();
        }

        public virtual async Task CommitTransactionAsync(CancellationToken token = default)
        {
            if (_session == null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _session.CommitTransactionAsync(token);
            _session.Dispose();
            _session = null;
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken token = default)
        {
            if (_session == null)
                throw new InvalidOperationException("Transaction has not been started.");

            await _session.AbortTransactionAsync();
            _session.Dispose();
            _session = null;
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
            _session?.Dispose();
        }
    }
}
