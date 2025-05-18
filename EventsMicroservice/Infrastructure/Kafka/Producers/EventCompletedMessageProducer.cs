using Application.Messages;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Kafka.Contracts.Events;
using Shared.Kafka.Producers.Common.Interfaces;

namespace Infrastructure.Kafka.Producers
{
    public class EventCompletedMessageProducer : IEventCompletedMessageProducer
    {
        private readonly IKafkaMessageProducer<EventCompletedDto> _producer;
        private readonly string _topic;

        public EventCompletedMessageProducer(IKafkaMessageProducer<EventCompletedDto> producer, IOptions<KafkaServerConfig> config)
        {
            _producer = producer;
            _topic = config.Value.EventCompleted;
        }

        public Task PublishAsync(EventCompletedDto message, CancellationToken cancellationToken = default)
        {
            return _producer.ProduceAsync(_topic, message, cancellationToken);
        }
    }
}
