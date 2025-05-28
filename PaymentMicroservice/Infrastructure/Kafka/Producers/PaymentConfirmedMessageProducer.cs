using Application.Messages;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Kafka.Contracts.Payment;
using Shared.Kafka.Producers.Common.Interfaces;

namespace Infrastructure.Kafka.Producers
{
    public class PaymentConfirmedMessageProducer : IPaymentConfirmedMessageProducer
    {
        private readonly IKafkaMessageProducer<PaymentConfirmedDto> _producer;
        private readonly string _topic;

        public PaymentConfirmedMessageProducer(IKafkaMessageProducer<PaymentConfirmedDto> producer, IOptions<KafkaServerConfig> config)
        {
            _producer = producer;
            _topic = config.Value.PaymentConfirmed;
        }

        public Task PublishAsync(PaymentConfirmedDto message, CancellationToken cancellationToken = default)
        {
            return _producer.ProduceAsync(_topic, message, cancellationToken);
        }
    }
}
