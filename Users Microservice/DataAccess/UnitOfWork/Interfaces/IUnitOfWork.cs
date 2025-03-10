using DataAccess.Repositories.Interfaces;

namespace DataAccess.UnitOfWork.Interfaces
{
    internal interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        IUserTransactionRepository UserTransactionRepository { get; }

        Task SaveChangesAsync();
    }
}
