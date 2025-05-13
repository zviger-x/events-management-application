using Infrastructure.Kafka.MessageHandlers.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Infrastructure.Kafka.MessageHandlers
{
    public class EventUpcomingMessageHandler : IEventUpcomingMessageHandler
    {
        public string Topic { get; init; }

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventUpcomingMessageHandler(IOptions<KafkaServerConfig> kafkaConfig, IServiceScopeFactory serviceScopeFactory)
        {
            Topic = kafkaConfig.Value.EventUpcoming;

            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task HandleMessage(string message, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

#warning TODO: EventUpcomingMessageHandler
            throw new NotImplementedException(nameof(EventUpcomingMessageHandler));
        }
    }
}
