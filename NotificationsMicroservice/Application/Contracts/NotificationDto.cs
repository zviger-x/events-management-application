using Shared.Enums;

namespace Application.Contracts
{
    public class NotificationDto
    {
        public Guid InCacheId { get; } = Guid.NewGuid();

        public Guid UserId { get; init; }

        public string Message { get; init; } = string.Empty;

        public DateTime DateTime { get; } = DateTime.UtcNow;

        public NotificationStatuses Status { get; } = NotificationStatuses.Pending;
    }
}
