using Shared.Kafka.Contracts.Events;

namespace Application.Messages
{
    public interface IEventUpcomingMessageProducer
    {
        Task PublishAsync(EventUpcomingDto message, CancellationToken cancellationToken = default);
    }
}
