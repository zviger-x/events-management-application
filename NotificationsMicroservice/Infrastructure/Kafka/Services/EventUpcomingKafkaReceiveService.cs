using Infrastructure.Kafka.MessageHandlers.Interfaces;
using Infrastructure.Kafka.Services.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Infrastructure.Kafka.Services
{
    public class EventUpcomingKafkaReceiveService : BaseKafkaService<IEventUpcomingMessageHandler>
    {
        public EventUpcomingKafkaReceiveService(
            IEventUpcomingMessageHandler kafkaMessageHandler,
            ILogger<EventUpcomingKafkaReceiveService> logger,
            IOptions<KafkaServerConfig> kafkaConfig)
            : base(kafkaMessageHandler, logger, kafkaConfig)
        {
        }
    }
}
