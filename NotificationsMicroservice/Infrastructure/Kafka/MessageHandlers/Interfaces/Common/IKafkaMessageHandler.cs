namespace Infrastructure.Kafka.MessageHandlers.Interfaces.Common
{
    public interface IKafkaMessageHandler
    {
        string Topic { get; init; }

        Task HandleMessage(string message, CancellationToken cancellationToken);
    }
}
