using Domain.Entities.Interfaces;

namespace Domain.Entities
{
    public class EventComment : IEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? EditedAt { get; set; }
    }
}
