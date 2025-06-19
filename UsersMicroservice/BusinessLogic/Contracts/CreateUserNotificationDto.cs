using Shared.Enums;

namespace BusinessLogic.Contracts
{
    public class CreateUserNotificationDto
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public NotificationStatuses Status { get; set; }
    }
}
