using AutoMapper;
using BusinessLogic.Exceptions;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Services
{
    public class UserNotificationService : BaseService<UserNotification>, IUserNotificationService
    {
        public UserNotificationService(IUnitOfWork unitOfWork, IMapper mapper, IUserNotificationValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public override async Task CreateAsync(UserNotification notification, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(notification, token);

            if (!await IsUserExistsAsync(notification.UserId, token))
                throw new ServiceValidationException(
                    UserNotificationValidationErrorCodes.UserIdIsInvalid,
                    UserNotificationValidationMessages.UserIdIsInvalid,
                    nameof(notification.UserId));

            notification.Id = default;
            await _unitOfWork.UserNotificationRepository.CreateAsync(notification, token);
        }

        public async Task<IEnumerable<UserNotification>> GetByUserIdAsync(Guid id, CancellationToken token = default)
        {
            return await _unitOfWork.UserNotificationRepository.GetByUserIdAsync(id, token);
        }

        private async Task<bool> IsUserExistsAsync(Guid guid, CancellationToken token = default)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(guid);

            return user != null;
        }
    }
}
