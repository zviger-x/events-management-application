using DataAccess.Enums;

namespace BusinessLogic.Contracts
{
    public class UpdateUserNotificationDto
    {
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public NotificationStatuses Status { get; set; }
    }
}
