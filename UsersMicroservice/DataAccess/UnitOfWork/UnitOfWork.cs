using DataAccess.Contexts;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.UnitOfWork
{

    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        private Lazy<IUserRepository> _userRepository;
        private Lazy<IUserNotificationRepository> _userNotificationRepository;
        private Lazy<IUserTransactionRepository> _userTransactionRepository;

        public UnitOfWork(UserDbContext context, IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {
            _userRepository = new Lazy<IUserRepository>(_serviceProvider.GetRequiredService<IUserRepository>);
            _userNotificationRepository = new Lazy<IUserNotificationRepository>(_serviceProvider.GetRequiredService<IUserNotificationRepository>);
            _userTransactionRepository = new Lazy<IUserTransactionRepository>(_serviceProvider.GetRequiredService<IUserTransactionRepository>);
        }

        public IUserRepository UserRepository => _userRepository.Value;

        public IUserNotificationRepository UserNotificationRepository => _userNotificationRepository.Value;

        public IUserTransactionRepository UserTransactionRepository => _userTransactionRepository.Value;

        public override void Dispose()
        {
            base.Dispose();

            if (_userRepository.IsValueCreated)
                _userRepository.Value.Dispose();

            if (_userNotificationRepository.IsValueCreated)
                _userNotificationRepository.Value.Dispose();

            if (_userTransactionRepository.IsValueCreated)
                _userTransactionRepository.Value.Dispose();
        }
    }
}
