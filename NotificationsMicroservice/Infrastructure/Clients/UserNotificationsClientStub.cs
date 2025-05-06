using Application.Clients;
using Application.Contracts;

namespace Infrastructure.Clients
{
    public class UserNotificationsClientStub : IUserNotificationsClient
    {
        public async Task<bool> TrySaveNotificationAsync(NotificationDto notification, CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);

            if (Random.Shared.NextDouble() < 30 / 100d)
                return await Task.FromResult(false);

            return await Task.FromResult(true);
        }
    }
}
