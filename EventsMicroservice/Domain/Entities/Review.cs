using Domain.Entities.Interfaces;

#pragma warning disable CS8618
namespace Domain.Entities
{
    public class Review : IEntity
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
