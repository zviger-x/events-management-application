using Infrastructure.Kafka.MessageHandlers.Interfaces;
using Infrastructure.Kafka.Services.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Infrastructure.Kafka.Services
{
    public class EventCompletedKafkaReceiveService : BaseKafkaService<IEventCompletedMessageHandler>
    {
        public EventCompletedKafkaReceiveService(
            IEventCompletedMessageHandler kafkaMessageHandler,
            ILogger<EventCompletedKafkaReceiveService> logger,
            IOptions<KafkaServerConfig> kafkaConfig)
            : base(kafkaMessageHandler, logger, kafkaConfig)
        {
        }
    }
}
