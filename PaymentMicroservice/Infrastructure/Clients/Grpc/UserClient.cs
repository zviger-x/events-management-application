using Application.Clients;
using Application.Contracts;
using AutoMapper;
using Shared.Grpc.User;

namespace Infrastructure.Clients.Grpc
{
    public class UserClient : IUserClient
    {
        private readonly IMapper _mapper;
        private readonly UserService.UserServiceClient _userServiceClient;

        public UserClient(IMapper mapper, UserService.UserServiceClient userServiceClient)
        {
            _mapper = mapper;
            _userServiceClient = userServiceClient;
        }

        public async Task<bool> CreateTransactionAsync(CreateUserTransactionDto transaction, CancellationToken cancellationToken = default)
        {
            var request = _mapper.Map<CreateTransactionRequest>(transaction);

            var result = await _userServiceClient.CreateTransactionAsync(request, cancellationToken: cancellationToken);

            return result.Success;
        }
    }
}
