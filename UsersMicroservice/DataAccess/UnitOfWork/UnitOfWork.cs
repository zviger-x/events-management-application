using DataAccess.Contexts;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.UnitOfWork
{
    public class UnitOfWOrk : IUnitOfWork
    {
        private readonly UserDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly IServiceProvider _serviceProvider;

        private Lazy<IUserRepository> _userRepository;
        private Lazy<IUserNotificationRepository> _userNotificationRepository;
        private Lazy<IUserTransactionRepository> _userTransactionRepository;

        public UnitOfWOrk(UserDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;

            _userRepository = new Lazy<IUserRepository>(() => _serviceProvider.GetRequiredService<IUserRepository>());
            _userNotificationRepository = new Lazy<IUserNotificationRepository>(() => _serviceProvider.GetRequiredService<IUserNotificationRepository>());
            _userTransactionRepository = new Lazy<IUserTransactionRepository>(() => _serviceProvider.GetRequiredService<IUserTransactionRepository>());
        }
        public IUserRepository UserRepository => _userRepository.Value;

        public IUserNotificationRepository UserNotificationRepository => _userNotificationRepository.Value;

        public IUserTransactionRepository UserTransactionRepository => _userTransactionRepository.Value;

        public async Task SaveChangesAsync(CancellationToken token = default)
        {
            await _context.SaveChangesAsync(token);
        }

        public async Task BeginTransactionAsync(CancellationToken token = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(token);
        }

        public async Task CommitTransactionAsync(CancellationToken token = default)
        {
            await _context.SaveChangesAsync(token);
            await _transaction.CommitAsync(token);
        }

        public async Task RollbackTransactionAsync(CancellationToken token = default)
        {
            await _transaction.RollbackAsync(token);
        }

        public async Task InvokeWithTransactionAsync(Func<CancellationToken, Task> action, CancellationToken token = default)
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

        public void Dispose()
        {
            _context?.Dispose();
            _transaction?.Dispose();
            
            if (_userRepository.IsValueCreated)
                _userRepository.Value.Dispose();

            if (_userNotificationRepository.IsValueCreated)
                _userNotificationRepository.Value.Dispose();

            if (_userTransactionRepository.IsValueCreated)
                _userTransactionRepository.Value.Dispose();
        }
    }
}
