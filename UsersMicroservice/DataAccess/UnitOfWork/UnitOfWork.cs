using DataAccess.Contexts;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.UnitOfWork
{
    internal class UnitOfWOrk : IUnitOfWork
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            _transaction?.Dispose();
            _userRepository.Dispose();
            _userNotificationRepository.Dispose();
            _userTransactionRepository.Dispose();
        }
    }
}
