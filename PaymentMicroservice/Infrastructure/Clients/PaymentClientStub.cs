using Application.Clients;

namespace Infrastructure.Clients
{
    public class PaymentClientStub : IPaymentClient
    {
        public async Task<bool> ProcessPaymentAsync(string token, float amount)
        {
            await Task.Delay(500);

            return await Task.FromResult(true);
        }
    }
}
