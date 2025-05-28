namespace Shared.Kafka.Contracts.Events
{
    public class EventUpcomingDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public IEnumerable<Guid> TargetUsers { get; set; }
    }
}
