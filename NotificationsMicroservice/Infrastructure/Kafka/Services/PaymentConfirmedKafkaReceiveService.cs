using Infrastructure.Kafka.MessageHandlers.Interfaces;
using Infrastructure.Kafka.Services.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Infrastructure.Kafka.Services
{
    public class PaymentConfirmedKafkaReceiveService : BaseKafkaService<IPaymentConfirmedMessageHandler>
    {
        public PaymentConfirmedKafkaReceiveService(
            IPaymentConfirmedMessageHandler kafkaMessageHandler,
            ILogger<PaymentConfirmedKafkaReceiveService> logger,
            IOptions<KafkaServerConfig> kafkaConfig)
            : base(kafkaMessageHandler, logger, kafkaConfig)
        {
        }
    }
}
