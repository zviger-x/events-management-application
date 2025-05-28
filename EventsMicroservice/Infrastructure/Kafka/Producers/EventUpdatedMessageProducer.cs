using Application.Messages;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Kafka.Contracts.Events;
using Shared.Kafka.Producers.Common.Interfaces;

namespace Infrastructure.Kafka.Producers
{
    public class EventUpdatedMessageProducer : IEventUpdatedMessageProducer
    {
        private readonly IKafkaMessageProducer<EventUpdatedDto> _producer;
        private readonly string _topic;

        public EventUpdatedMessageProducer(IKafkaMessageProducer<EventUpdatedDto> producer, IOptions<KafkaServerConfig> config)
        {
            _producer = producer;
            _topic = config.Value.EventUpdated;
        }

        public Task PublishAsync(EventUpdatedDto message, CancellationToken cancellationToken = default)
        {
            return _producer.ProduceAsync(_topic, message, cancellationToken);
        }
    }
}
