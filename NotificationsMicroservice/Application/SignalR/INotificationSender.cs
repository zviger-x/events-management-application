using Application.Contracts;

namespace Application.SignalR
{
    public interface INotificationSender
    {
        /// <summary>
        /// Attempts to save a notification.
        /// </summary>
        /// <param name="notification">The notification to be saved.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if saving was successful; otherwise, false.</returns>
        Task SendAsync(NotificationDto notification, CancellationToken cancellationToken);
    }
}
