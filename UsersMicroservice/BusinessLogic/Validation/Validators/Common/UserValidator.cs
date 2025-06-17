using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using DataAccess.Entities;
using FluentValidation;

namespace BusinessLogic.Validation.Validators.Common
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.Name)
                .NotNull()
                    .WithMessage(UserValidationMessages.NameIsNull)
                    .WithErrorCode(UserValidationErrorCodes.NameIsNull)
                .NotEmpty()
                    .WithMessage(UserValidationMessages.NameIsEmpty)
                    .WithErrorCode(UserValidationErrorCodes.NameIsEmpty);

            RuleFor(u => u.Surname)
                .NotNull()
                    .WithMessage(UserValidationMessages.SurnameIsNull)
                    .WithErrorCode(UserValidationErrorCodes.SurnameIsNull)
                .NotEmpty()
                    .WithMessage(UserValidationMessages.SurnameIsEmpty)
                    .WithErrorCode(UserValidationErrorCodes.SurnameIsEmpty);

            RuleFor(u => u.Email)
                .NotNull()
                    .WithMessage(UserValidationMessages.EmailIsNull)
                    .WithErrorCode(UserValidationErrorCodes.EmailIsNull)
                .NotEmpty()
                    .WithMessage(UserValidationMessages.EmailIsEmpty)
                    .WithErrorCode(UserValidationErrorCodes.EmailIsEmpty)
                .EmailAddress()
                    .WithMessage(UserValidationMessages.EmailIsInvalid)
                    .WithErrorCode(UserValidationErrorCodes.EmailIsInvalid);
        }
    }
}
