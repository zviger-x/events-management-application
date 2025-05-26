using BusinessLogic.Contracts;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class UpdateUserNotificationDtoValidator : AbstractValidator<UpdateUserNotificationDto>
    {
        public UpdateUserNotificationDtoValidator()
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
        }
    }
}
