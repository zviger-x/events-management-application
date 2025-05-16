using Infrastructure.Kafka.MessageHandlers.Interfaces;
using Infrastructure.Kafka.Services.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Infrastructure.Kafka.Services
{
    public class EventUpdatedKafkaReceiveService : BaseKafkaService<IEventUpdatedMessageHandler>
    {
        public EventUpdatedKafkaReceiveService(
            IEventUpdatedMessageHandler kafkaMessageHandler,
            ILogger<EventUpdatedKafkaReceiveService> logger,
            IOptions<KafkaServerConfig> kafkaConfig)
            : base(kafkaMessageHandler, logger, kafkaConfig)
        {
        }
    }
}
