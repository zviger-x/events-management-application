using Application.Contracts;
using Application.MediatR.Commands;
using Infrastructure.Kafka.MessageHandlers.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using System.Text.Json;

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

            var model = JsonSerializer.Deserialize<StubModel>(message);

            foreach (var user in model.Users)
            {
                var command = new SendNotificationCommand
                {
                    Notification = new NotificationDto
                    {
                        UserId = user,
                        Message = $"\"Тестовое\" уведомление о изменении ивента{Environment.NewLine}Для пользователя: {{{user}}}{Environment.NewLine}Название ивента: {model.Name}"
                    },
                    ThrowError = !model.IsValid
                };

                await mediator.Send(command, cancellationToken);
            }
        }

        private class StubModel
        {
            public string Name { get; set; }
            public Guid[] Users { get; set; }
            public bool IsValid { get; set; }
        }
    }
}
