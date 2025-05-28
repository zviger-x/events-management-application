namespace Shared.Kafka.Contracts.Events
{
    public class EventCompletedDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CompletedAt { get; set; }
        public IEnumerable<Guid> TargetUsers { get; set; }
    }
}
