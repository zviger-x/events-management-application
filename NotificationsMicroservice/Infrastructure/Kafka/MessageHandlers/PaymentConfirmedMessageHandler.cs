using Application.Contracts;
using Application.MediatR.Commands;
using Infrastructure.Kafka.MessageHandlers.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.KafkaContracts.Payment;
using System.Text.Json;

namespace Infrastructure.Kafka.MessageHandlers
{
    public class PaymentConfirmedMessageHandler : IPaymentConfirmedMessageHandler
    {
        public string Topic { get; init; }

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PaymentConfirmedMessageHandler(IOptions<KafkaServerConfig> kafkaConfig, IServiceScopeFactory serviceScopeFactory)
        {
            Topic = kafkaConfig.Value.PaymentConfirmed;

            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task HandleMessage(string message, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var dto = JsonSerializer.Deserialize<PaymentConfirmedDto>(message);

            var notification = new NotificationDto
            {
                UserId = dto.TargetUser,
                Message = GetMessage(dto.EventName, dto.ConfirmedAt, dto.Amount),
                Metadata = new Dictionary<string, string>
                {
                    { nameof(dto.EventId), dto.EventId.ToString() }
                }
            };

            var commend = new SendNotificationCommand { Notification = notification };

            await mediator.Send(commend, cancellationToken);
        }

        private static string GetMessage(string eventName, DateTime confirmedAt, float amount)
        {
            return $"Оплата за участие в мероприятии \"{eventName}\" была успешно подтверждена " +
                   $"{confirmedAt:dd.MM.yyyy в HH:mm}. Сумма: {amount:0.00}. Спасибо за вашу регистрацию!";
        }
    }
}
