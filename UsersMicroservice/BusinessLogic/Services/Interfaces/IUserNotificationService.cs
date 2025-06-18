using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces.Common;
using DataAccess.Entities;

namespace BusinessLogic.Services.Interfaces
{
    public interface IUserNotificationService : ICreateService<CreateUserNotificationDto>,
        IUpdateService<UpdateUserNotificationDto>,
        IReadService<UserNotification>,
        IDeleteService
    {
        /// <summary>
        /// Returns a collection of user notifications by the specified user's ID.  
        /// Access is granted only if the requesting user is the same as the target user, or if the requester has administrative privileges.
        /// </summary>
        /// <param name="targetUserId">The ID of the user whose notifications are being requested.</param>
        /// <param name="currentUserId">The ID of the currently authenticated user making the request.</param>
        /// <param name="isAdmin">Specifies whether the current user has administrative privileges.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A collection of user notifications.</returns>
        Task<IEnumerable<UserNotification>> GetByUserIdAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, CancellationToken token = default);
    }
}
