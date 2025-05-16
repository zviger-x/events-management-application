using Application.Contracts;
using Application.SignalR;
using Microsoft.AspNetCore.SignalR;
using NotificationsAPI.SignalR.Hubs;

namespace NotificationsAPI.SignalR.Senders
{
    public class NotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationSender(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendAsync(NotificationDto notification, CancellationToken cancellationToken)
        {
            var hubMethod = NotificationHub.Methods.Notify;
            var userId = notification.UserId.ToString();

            await _hubContext.Clients.User(userId).SendAsync(hubMethod, notification, cancellationToken);
        }
    }
}
