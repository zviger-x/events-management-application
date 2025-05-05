using Application.Contracts;

namespace Application.Clients
{
    public interface IUserNotificationsClient
    {
        /// <summary>
        /// Sends a notification to the user.
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task<bool> TrySaveNotificationAsync(NotificationDto notification, CancellationToken cancellationToken);
    }
}
