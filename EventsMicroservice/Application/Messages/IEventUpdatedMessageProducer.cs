using Shared.Kafka.Contracts.Events;

namespace Application.Messages
{
    public interface IEventUpdatedMessageProducer
    {
        Task PublishAsync(EventUpdatedDto message, CancellationToken cancellationToken = default);
    }
}
