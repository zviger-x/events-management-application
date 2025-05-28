using Application.Clients;
using Application.Contracts;
using AutoMapper;
using Shared.Grpc.User;

namespace Infrastructure.Clients.Grpc
{
    public class UserNotificationsClient : IUserNotificationsClient
    {
        private readonly IMapper _mapper;
        private readonly UserService.UserServiceClient _userServiceClient;

        public UserNotificationsClient(IMapper mapper, UserService.UserServiceClient userServiceClient)
        {
            _mapper = mapper;
            _userServiceClient = userServiceClient;
        }

        public async Task<bool> TrySaveNotificationAsync(NotificationDto notification, CancellationToken cancellationToken)
        {
            var request = _mapper.Map<CreateNotificationRequest>(notification);

            var result = await _userServiceClient.CreateNotificationAsync(request, cancellationToken: cancellationToken);

            return result.Success;
        }
    }
}
