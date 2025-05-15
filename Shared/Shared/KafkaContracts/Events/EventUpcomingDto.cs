namespace Shared.KafkaContracts.Events
{
    public class EventUpcomingDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public IEnumerable<Guid> TargetUsers { get; set; }
    }
}
