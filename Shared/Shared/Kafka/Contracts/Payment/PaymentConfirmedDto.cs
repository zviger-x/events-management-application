namespace Shared.Kafka.Contracts.Payment
{
    public class PaymentConfirmedDto
    {
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public float Amount { get; set; }
        public DateTime ConfirmedAt { get; set; }
        public Guid TargetUser { get; set; }
    }
}
