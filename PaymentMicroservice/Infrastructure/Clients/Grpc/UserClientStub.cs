using Application.Clients;
using Application.Contracts;

namespace Infrastructure.Clients.Grpc
{
    internal class UserClientStub : IUserClient
    {
        public Task<bool> CreateTransactionAsync(CreateUserTransactionDto transaction)
        {
            return Task.FromResult(true);
        }
    }
}
