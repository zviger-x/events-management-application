using Application.Clients;
using Application.Contracts;

namespace Infrastructure.Clients
{
    public class UserClientStub : IUserClient
    {
        public async Task<bool> CreateTransactionAsync(CreateUserTransactionDto transaction)
        {
            await Task.Delay(500);

            return await Task.FromResult(true);
        }
    }
}
