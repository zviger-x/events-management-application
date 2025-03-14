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
                .NotNull().WithMessage(UserNotificationValidationMessages.MessageNotNull)
                .NotEmpty().WithMessage(UserNotificationValidationMessages.MessageNotEmpty);

            RuleFor(n => n.DateTime)
                .NotNull().WithMessage(UserNotificationValidationMessages.DateTimeNotNull);

            // Нужно в будущем добавить проверку на наличие
            RuleFor(n => n.UserId)
                .NotNull().WithMessage(UserNotificationValidationMessages.UserIdNotNull)
                .NotEmpty().WithMessage(UserNotificationValidationMessages.UserIdNotEmpty);
        }
    }
}
