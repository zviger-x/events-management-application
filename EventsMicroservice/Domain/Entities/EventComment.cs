using Shared.Entities.Interfaces;

namespace Domain.Entities
{
    public class EventComment : IEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset? EditedAt { get; set; }
    }
}
