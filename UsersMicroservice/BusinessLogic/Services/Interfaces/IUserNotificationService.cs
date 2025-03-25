using DataAccess.Entities;

namespace BusinessLogic.Services.Interfaces
{
    public interface IUserNotificationService : IService<UserNotification>
    {
        /// <summary>
        /// Returns a collection of user notifications by the user's id.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A collection of user notifications.</returns>
        Task<IEnumerable<UserNotification>> GetByUserIdAsync(Guid id, CancellationToken token = default);
    }
}
