using Application.Contracts;
using Application.MediatR.Commands;
using Infrastructure.Kafka.MessageHandlers.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.KafkaContracts.Events;
using System.Text.Json;

namespace Infrastructure.Kafka.MessageHandlers
{
    public class EventCompletedMessageHandler : IEventCompletedMessageHandler
    {
        public string Topic { get; init; }

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventCompletedMessageHandler(IOptions<KafkaServerConfig> kafkaConfig, IServiceScopeFactory serviceScopeFactory)
        {
            Topic = kafkaConfig.Value.EventCompleted;

            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleMessage(string message, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var dto = JsonSerializer.Deserialize<EventCompletedDto>(message);

            foreach (var user in dto.TargetUsers)
            {
                var notification = new NotificationDto
                {
                    UserId = user,
                    Message = GetMessage(dto.Name, dto.CompletedAt),
                    Metadata = new Dictionary<string, string>
                    {
                        { nameof(dto.EventId), dto.EventId.ToString() }
                    }
                };

                var commend = new SendNotificationCommand { Notification = notification };

                await mediator.Send(commend, cancellationToken);
            }
        }

        private static string GetMessage(string eventName, DateTime completedAt)
        {
            return $"Мероприятие \"{eventName}\" завершилось {completedAt:dd.MM.yyyy в HH:mm}. " +
                   "Не забудьте оставить свой отзыв или комментарий. Спасибо, что были с нами!";
        }
    }
}
