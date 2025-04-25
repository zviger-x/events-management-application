using Application.Clients;

namespace Infrastructure.Clients
{
    internal class PaymentClientStub : IPaymentClient
    {
        public Task<bool> ProcessPaymentAsync(string token, float amount)
        {
            return Task.FromResult(true);
        }
    }
}
