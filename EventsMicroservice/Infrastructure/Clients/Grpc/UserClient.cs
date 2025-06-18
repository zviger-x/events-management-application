using Application.Clients;
using Shared.Grpc.User;

namespace Infrastructure.Clients.Grpc
{
    public class UserClient : IUserClient
    {
        private readonly UserService.UserServiceClient _userServiceClient;

        public UserClient(UserService.UserServiceClient userServiceClient)
        {
            _userServiceClient = userServiceClient;
        }

        public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var request = new UserExistsRequest() { UserId = userId.ToString() };

            var result = await _userServiceClient.UserExistsAsync(request, cancellationToken: cancellationToken);

            return result.Exists;
        }
    }
}
