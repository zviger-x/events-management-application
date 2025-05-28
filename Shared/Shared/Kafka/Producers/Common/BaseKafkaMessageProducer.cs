using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Kafka.Producers.Common.Interfaces;
using System.Text.Json;

namespace Shared.Kafka.Producers.Common
{
    public class BaseKafkaMessageProducer<T> : IKafkaMessageProducer<T>
    {
        private readonly IProducer<Null, string> _producer;

        public BaseKafkaMessageProducer(IOptions<KafkaServerConfig> config)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config.Value.BootstrapServers,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public async Task ProduceAsync(string topic, T message, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json }, cancellationToken);
        }
    }
}