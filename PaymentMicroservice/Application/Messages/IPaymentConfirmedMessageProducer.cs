using Shared.Kafka.Contracts.Payment;

namespace Application.Messages
{
    public interface IPaymentConfirmedMessageProducer
    {
        Task PublishAsync(PaymentConfirmedDto message, CancellationToken cancellationToken = default);
    }
}
