namespace Shared.Kafka.Contracts.Events
{
    public class EventUpdatedDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<Guid> TargetUsers { get; set; }
    }
}
