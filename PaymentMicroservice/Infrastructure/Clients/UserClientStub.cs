using Application.Clients;
using Application.Contracts;

namespace Infrastructure.Clients
{
    public class UserClientStub : IUserClient
    {
        public async Task<bool> CreateTransactionAsync(CreateUserTransactionDto transaction, CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken);

            return await Task.FromResult(true);
        }
    }
}
