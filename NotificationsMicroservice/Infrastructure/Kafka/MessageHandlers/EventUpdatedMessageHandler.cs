using Application.Contracts;
using Application.MediatR.Commands;
using Infrastructure.Common;
using Infrastructure.Kafka.MessageHandlers.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.KafkaContracts.Events;
using System.Text.Json;

namespace Infrastructure.Kafka.MessageHandlers
{
    public class EventUpdatedMessageHandler : IEventUpdatedMessageHandler
    {
        public string Topic { get; init; }

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventUpdatedMessageHandler(IOptions<KafkaServerConfig> kafkaConfig, IServiceScopeFactory serviceScopeFactory)
        {
            Topic = kafkaConfig.Value.EventUpdated;

            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleMessage(string message, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var dto = JsonSerializer.Deserialize<EventUpdatedDto>(message);

            var notificationMessage = NotificationMessageFactory.EventUpdated(dto.Name, dto.UpdatedAt);

            foreach (var user in dto.TargetUsers)
            {
                var notification = new NotificationDto
                {
                    UserId = user,
                    Message = notificationMessage,
                    Metadata = new Dictionary<string, string>
                    {
                        { nameof(dto.EventId), dto.EventId.ToString() }
                    }
                };

                var commend = new SendNotificationCommand { Notification = notification };

                await mediator.Send(commend, cancellationToken);
            }
        }
    }
}
