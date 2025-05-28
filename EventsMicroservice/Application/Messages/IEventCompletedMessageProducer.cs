using Shared.Kafka.Contracts.Events;

namespace Application.Messages
{
    public interface IEventCompletedMessageProducer
    {
        Task PublishAsync(EventCompletedDto message, CancellationToken cancellationToken = default);
    }
}
