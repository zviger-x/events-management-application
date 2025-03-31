using Domain.Entities.Interfaces;

namespace Domain.Entities
{
    public class Review : IEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; } = default!;
        public DateTime CreationTime { get; set; }
    }
}
