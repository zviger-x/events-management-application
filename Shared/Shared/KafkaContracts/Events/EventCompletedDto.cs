namespace Shared.KafkaContracts.Events
{
    public class EventCompletedDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTime CompletedAt { get; set; }
        public IEnumerable<Guid> TargetUsers { get; set; }
    }
}
