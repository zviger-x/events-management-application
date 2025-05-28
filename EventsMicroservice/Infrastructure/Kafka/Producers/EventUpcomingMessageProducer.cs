using Application.Messages;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Kafka.Contracts.Events;
using Shared.Kafka.Producers.Common.Interfaces;

namespace Infrastructure.Kafka.Producers
{
    public class EventUpcomingMessageProducer : IEventUpcomingMessageProducer
    {
        private readonly IKafkaMessageProducer<EventUpcomingDto> _producer;
        private readonly string _topic;

        public EventUpcomingMessageProducer(IKafkaMessageProducer<EventUpcomingDto> producer, IOptions<KafkaServerConfig> config)
        {
            _producer = producer;
            _topic = config.Value.EventUpcoming;
        }

        public Task PublishAsync(EventUpcomingDto message, CancellationToken cancellationToken = default)
        {
            return _producer.ProduceAsync(_topic, message, cancellationToken);
        }
    }
}
