using Infrastructure.Kafka.MessageHandlers.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Infrastructure.Kafka.MessageHandlers
{
    public class PaymentConfirmedMessageHandler : IPaymentConfirmedMessageHandler
    {
        public string Topic { get; init; }

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PaymentConfirmedMessageHandler(IOptions<KafkaServerConfig> kafkaConfig, IServiceScopeFactory serviceScopeFactory)
        {
            Topic = kafkaConfig.Value.EventCompleted;

            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task HandleMessage(string message, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

#warning TODO: PaymentConfirmedMessageHandler
            throw new NotImplementedException(nameof(PaymentConfirmedMessageHandler));
        }
    }
}
