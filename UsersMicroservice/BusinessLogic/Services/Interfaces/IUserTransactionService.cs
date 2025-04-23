using BusinessLogic.Services.Interfaces.Common;
using DataAccess.Entities;

namespace BusinessLogic.Services.Interfaces
{
    public interface IUserTransactionService : ICreateService<UserTransaction>,
        IUpdateService<UserTransaction>,
        IReadService<UserTransaction>,
        IDeleteService
    {
        /// <summary>
        /// Returns a collection of user transactions by the user's id.
        /// </summary>
        /// <param name="targetUserId">The ID of the user to retrieve transactions.</param>
        /// <param name="currentUserId">The ID of the current user performing the action.</param>
        /// <param name="isAdmin">Indicates whether the current user has administrative privileges.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A collection of user transactions.</returns>
        Task<IEnumerable<UserTransaction>> GetByUserIdAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, CancellationToken token = default);
    }
}
