namespace Shared.Configuration
{
    public class KafkaServerConfig
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public Dictionary<string, string> Topics { get; set; }

        public string EventStub => Topics["EventStub"];
        public string EventUpcoming => Topics["EventUpcoming"];
        public string EventUpdated => Topics["EventUpdated"];
        public string EventCompleted => Topics["EventCompleted"];
        public string PaymentConfirmed => Topics["PaymentConfirmed"];
    }
}