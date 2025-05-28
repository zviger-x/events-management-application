using Shared.Entities.Interfaces;
using Shared.Enums;

namespace DataAccess.Entities
{
    public class UserNotification : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public NotificationStatuses Status { get; set; }
    }
}
