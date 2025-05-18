using BusinessLogic.Services.Interfaces;
using Grpc.Core;
using Shared.Grpc.User;
using GrpcUserService = Shared.Grpc.User.UserService;

namespace UsersAPI.Services
{
    public class UserService : GrpcUserService.UserServiceBase
    {
        private readonly IUserService _userService;

        public UserService(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task<UserExistsResponse> UserExists(UserExistsRequest request, ServerCallContext context)
        {
            var isUserExists = await _userService.UserExists(Guid.Parse(request.UserId), context.CancellationToken);

            var response = new UserExistsResponse() { Exists = isUserExists };

            return response;
        }
    }
}
