using Application.Clients;
using Application.Contracts;

namespace Infrastructure.Clients
{
    public class UserNotificationsClientStub : IUserNotificationsClient
    {
        public async Task<bool> TrySaveNotificationAsync(NotificationDto notification, CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);

            return await Task.FromResult(true);
        }
    }
}
