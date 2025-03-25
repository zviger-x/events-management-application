using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class UserNotificationValidator : BaseValidator<UserNotification>, IUserNotificationValidator
    {
        public UserNotificationValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            RuleFor(n => n.Message)
                .NotNull()
                    .WithMessage(UserNotificationValidationMessages.MessageIsNull)
                    .WithErrorCode(UserNotificationValidationErrorCodes.MessageIsNull)
                .NotEmpty()
                    .WithMessage(UserNotificationValidationMessages.MessageIsEmpty)
                    .WithErrorCode(UserNotificationValidationErrorCodes.MessageIsEmpty);

            RuleFor(n => n.DateTime)
                .NotNull()
                    .WithMessage(UserNotificationValidationMessages.DateTimeIsNull)
                    .WithErrorCode(UserNotificationValidationErrorCodes.DateTimeIsNull);

            RuleFor(n => n.UserId)
                .NotNull()
                    .WithMessage(UserNotificationValidationMessages.UserIdIsNull)
                    .WithErrorCode(UserNotificationValidationErrorCodes.UserIdIsNull)
                .NotEmpty()
                    .WithMessage(UserNotificationValidationMessages.UserIdIsEmpty)
                    .WithErrorCode(UserNotificationValidationErrorCodes.UserIdIsEmpty)
                .MustAsync(IsUserExists)
                    .WithMessage(UserNotificationValidationMessages.UserIdIsInvalid)
                    .WithErrorCode(UserNotificationValidationErrorCodes.UserIdIsInvalid);
        }

        private async Task<bool> IsUserExists(Guid guid, CancellationToken token)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(guid);

            return user != null;
        }
    }
}
