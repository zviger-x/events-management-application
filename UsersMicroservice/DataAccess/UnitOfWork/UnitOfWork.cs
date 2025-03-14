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

        private IUserRepository _userRepository;
        private IUserNotificationRepository _userNotificationRepository;
        private IUserTransactionRepository _userTransactionRepository;

        public UnitOfWOrk(UserDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public IUserRepository UserRepository
        {
            get => _userRepository ??= _serviceProvider.GetRequiredService<IUserRepository>();
        }

        public IUserNotificationRepository UserNotificationRepository
        {
            get => _userNotificationRepository ??= _serviceProvider.GetRequiredService<IUserNotificationRepository>();
        }

        public IUserTransactionRepository UserTransactionRepository
        {
            get => _userTransactionRepository ??= _serviceProvider.GetRequiredService<IUserTransactionRepository>();
        }

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
            _userRepository?.Dispose();
            _userNotificationRepository?.Dispose();
            _userTransactionRepository?.Dispose();
        }
    }
}
