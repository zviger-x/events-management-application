using BusinessLogic.Contracts;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(u => u.CurrentPassword)
                .NotNull()
                    .WithMessage(ChangePasswordValidationMessages.CurrentPasswordIsNull)
                    .WithErrorCode(ChangePasswordValidationErrorCodes.CurrentPasswordIsNull)
                .NotEmpty()
                    .WithMessage(ChangePasswordValidationMessages.CurrentPasswordIsEmpty)
                    .WithErrorCode(ChangePasswordValidationErrorCodes.CurrentPasswordIsEmpty);

            RuleFor(u => u.NewPassword)
                .NotNull()
                    .WithMessage(ChangePasswordValidationMessages.PasswordIsNull)
                    .WithErrorCode(ChangePasswordValidationErrorCodes.PasswordIsNull)
                .NotEmpty()
                    .WithMessage(ChangePasswordValidationMessages.PasswordIsEmpty)
                    .WithErrorCode(ChangePasswordValidationErrorCodes.PasswordIsEmpty);

            RuleFor(u => u.ConfirmPassword)
                .Equal(u => u.NewPassword)
                    .WithMessage(ChangePasswordValidationMessages.PasswordsDoNotMatch)
                    .WithErrorCode(ChangePasswordValidationErrorCodes.PasswordsDoNotMatch);
        }
    }
}
