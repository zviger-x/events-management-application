using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using Grpc.Core;
using Shared.Grpc.User;
using GrpcUserService = Shared.Grpc.User.UserService;

namespace UsersAPI.Services
{
    public class UserService : GrpcUserService.UserServiceBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUserTransactionService _userTransactionService;
        private readonly IUserNotificationService _userNotificationService;

        public UserService(
            IMapper mapper,
            IUserService userService,
            IUserTransactionService userTransactionService,
            IUserNotificationService userNotificationService)
        {
            _mapper = mapper;
            _userService = userService;
            _userTransactionService = userTransactionService;
            _userNotificationService = userNotificationService;
        }

        public override async Task<UserExistsResponse> UserExists(UserExistsRequest request, ServerCallContext context)
        {
            var isUserExists = await _userService.UserExists(Guid.Parse(request.UserId), context.CancellationToken);

            var response = new UserExistsResponse() { Exists = isUserExists };

            return response;
        }

        public override async Task<CreateTransactionResult> CreateTransaction(CreateTransactionRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<CreateUserTransactionDto>(request);

            var guid = await _userTransactionService.CreateAsync(dto, context.CancellationToken);

            return new CreateTransactionResult { Success = true };
        }

        public override async Task<CreateNotificationResult> CreateNotification(CreateNotificationRequest request, ServerCallContext context)
        {
            var dto = _mapper.Map<CreateUserNotificationDto>(request);

            var guid = await _userNotificationService.CreateAsync(dto, context.CancellationToken);

            return new CreateNotificationResult { Success = true };
        }
    }
}
