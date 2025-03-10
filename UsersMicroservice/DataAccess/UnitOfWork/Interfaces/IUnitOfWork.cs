using DataAccess.Repositories.Interfaces;

namespace DataAccess.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        IUserTransactionRepository UserTransactionRepository { get; }

        Task SaveChangesAsync();
    }
}
