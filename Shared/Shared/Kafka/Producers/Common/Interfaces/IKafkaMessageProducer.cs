namespace Shared.Kafka.Producers.Common.Interfaces
{
    public interface IKafkaMessageProducer<T>
    {
        Task ProduceAsync(string topic, T message, CancellationToken cancellationToken = default);
    }
}
