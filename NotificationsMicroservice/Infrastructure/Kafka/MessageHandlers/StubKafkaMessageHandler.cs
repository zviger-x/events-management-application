using Application.Contracts;
using Application.MediatR.Commands;
using Infrastructure.Kafka.MessageHandlers.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using System.Text.Json.Nodes;

namespace Infrastructure.Kafka.MessageHandlers
{
    public class StubKafkaMessageHandler : IStubKafkaMessageHandler
    {
        public string Topic { get; init; }

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public StubKafkaMessageHandler(IOptions<KafkaServerConfig> kafkaConfig, IServiceScopeFactory serviceScopeFactory)
        {
            Topic = kafkaConfig.Value.EventStub;

            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleMessage(string message, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var node = JsonNode.Parse(message);

            var name = node["name"]?.ToString();
            var users = node["users"]?.AsArray().Select(a => Guid.Parse(a.ToString())).ToList();

            foreach (var user in users)
            {
                var command = new SendNotificationCommand
                {
                    Notification = new NotificationDto
                    {
                        UserId = user,
                        Message = $"\"Тестовое\" уведомление о изменении ивента{Environment.NewLine}Для пользователя: {{{user}}}{Environment.NewLine}Название ивента: {name}"
                    }
                };

                await mediator.Send(command, cancellationToken);
            }
        }
    }
}
