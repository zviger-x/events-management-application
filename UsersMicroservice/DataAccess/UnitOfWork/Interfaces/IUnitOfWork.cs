using DataAccess.Repositories.Interfaces;
using Shared.UnitOfWork;

namespace DataAccess.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        IUserTransactionRepository UserTransactionRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
    }
}
