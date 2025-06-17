using BusinessLogic.Contracts;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class CreateUserNotificationDtoValidator : AbstractValidator<CreateUserNotificationDto>
    {
        public CreateUserNotificationDtoValidator()
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
                    .WithErrorCode(UserNotificationValidationErrorCodes.UserIdIsEmpty);
        }
    }
}
