using Microsoft.AspNetCore.SignalR;

namespace NotificationsAPI.SignalR.Hubs
{
    public class NotificationHub : Hub
    {
        public static class Methods
        {
            public const string Notify = "Notify";
        }
    }
}
