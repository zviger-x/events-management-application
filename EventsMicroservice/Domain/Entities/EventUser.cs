using Shared.Entities.Interfaces;

namespace Domain.Entities
{
    public class EventUser : IEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public Guid SeatId { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
