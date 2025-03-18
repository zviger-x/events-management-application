using DataAccess.Repositories.Interfaces;

namespace DataAccess.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        IUserTransactionRepository UserTransactionRepository { get; }
    }
}
