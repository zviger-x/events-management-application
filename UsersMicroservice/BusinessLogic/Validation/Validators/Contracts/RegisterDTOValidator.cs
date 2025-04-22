using BusinessLogic.Contracts;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(u => u.Name)
                .NotNull()
                    .WithMessage(RegisterValidationMessages.NameIsNull)
                    .WithErrorCode(RegisterValidationErrorCodes.NameIsNull)
                .NotEmpty()
                    .WithMessage(RegisterValidationMessages.NameIsEmpty)
                    .WithErrorCode(RegisterValidationErrorCodes.NameIsEmpty);

            RuleFor(u => u.Surname)
                .NotNull()
                    .WithMessage(RegisterValidationMessages.SurnameIsNull)
                    .WithErrorCode(RegisterValidationErrorCodes.SurnameIsNull)
                .NotEmpty()
                    .WithMessage(RegisterValidationMessages.SurnameIsEmpty)
                    .WithErrorCode(RegisterValidationErrorCodes.SurnameIsEmpty);

            RuleFor(u => u.Email)
                .NotNull()
                    .WithMessage(RegisterValidationMessages.EmailIsNull)
                    .WithErrorCode(RegisterValidationErrorCodes.EmailIsNull)
                .NotEmpty()
                    .WithMessage(RegisterValidationMessages.EmailIsEmpty)
                    .WithErrorCode(RegisterValidationErrorCodes.EmailIsEmpty)
                .EmailAddress()
                    .WithMessage(RegisterValidationMessages.EmailIsInvalid)
                    .WithErrorCode(RegisterValidationErrorCodes.EmailIsInvalid);

            RuleFor(u => u.Password)
                .NotNull()
                    .WithMessage(RegisterValidationMessages.PasswordIsNull)
                    .WithErrorCode(RegisterValidationErrorCodes.PasswordIsNull)
                .NotEmpty()
                    .WithMessage(RegisterValidationMessages.PasswordIsEmpty)
                    .WithErrorCode(RegisterValidationErrorCodes.PasswordIsEmpty);

            RuleFor(u => u.ConfirmPassword)
                .Equal(u => u.Password)
                    .WithMessage(RegisterValidationMessages.PasswordsDoNotMatch)
                    .WithErrorCode(RegisterValidationErrorCodes.PasswordsDoNotMatch);
        }
    }
}
