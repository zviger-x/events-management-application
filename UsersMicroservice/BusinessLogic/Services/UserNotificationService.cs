using AutoMapper;
using BusinessLogic.Exceptions;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;
using ValidationException = BusinessLogic.Exceptions.ValidationException;

namespace BusinessLogic.Services
{
    public class UserNotificationService : BaseService<UserNotification>, IUserNotificationService
    {
        private readonly ICurrentUserService _currentUserService;

        public UserNotificationService(IUnitOfWork unitOfWork, IMapper mapper, IUserNotificationValidator validator, ICurrentUserService currentUserService)
            : base(unitOfWork, mapper, validator)
        {
            _currentUserService = currentUserService;
        }

        public override async Task CreateAsync(UserNotification notification, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(notification, token);

            if (!await IsUserExistsAsync(notification.UserId, token))
                throw new ValidationException(
                    UserNotificationValidationErrorCodes.UserIdIsInvalid,
                    UserNotificationValidationMessages.UserIdIsInvalid,
                    nameof(notification.UserId));

            await _unitOfWork.UserNotificationRepository.CreateAsync(notification, token);
        }

        public async Task<IEnumerable<UserNotification>> GetByUserIdAsync(Guid id, CancellationToken token = default)
        {
            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isCurrentUser = currentUserId == id;

            if (!isCurrentUser && !isAdmin)
                throw new ForbiddenAccessException("You do not have permission to perform this action.");

            return await _unitOfWork.UserNotificationRepository.GetByUserIdAsync(id, token);
        }

        private async Task<bool> IsUserExistsAsync(Guid guid, CancellationToken token = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(guid, token);

            return user != null;
        }
    }
}
