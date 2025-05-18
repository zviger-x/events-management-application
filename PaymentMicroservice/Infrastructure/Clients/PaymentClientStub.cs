using Application.Clients;

namespace Infrastructure.Clients
{
    public class PaymentClientStub : IPaymentClient
    {
        public async Task<bool> ProcessPaymentAsync(string token, float amount, CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken);

            if (Random.Shared.NextDouble() > .5d)
                return false;

            return await Task.FromResult(true);
        }

        public async Task<bool> RefundPaymentAsync(string token, float amount, CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken);

            return await Task.FromResult(true);
        }
    }
}
