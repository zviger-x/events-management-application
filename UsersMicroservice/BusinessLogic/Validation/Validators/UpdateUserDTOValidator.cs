using BusinessLogic.Contracts;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>, IUpdateUserDTOValidator
    {
        public UpdateUserDTOValidator()
        {
            RuleFor(u => u.Name)
                .NotNull()
                    .WithMessage(UpdateUserValidationMessages.NameIsNull)
                    .WithErrorCode(UpdateUserValidationErrorCodes.NameIsNull)
                .NotEmpty()
                    .WithMessage(UpdateUserValidationMessages.NameIsEmpty)
                    .WithErrorCode(UpdateUserValidationErrorCodes.NameIsEmpty);

            RuleFor(u => u.Surname)
                .NotNull()
                    .WithMessage(UpdateUserValidationMessages.SurnameIsNull)
                    .WithErrorCode(UpdateUserValidationErrorCodes.SurnameIsNull)
                .NotEmpty()
                    .WithMessage(UpdateUserValidationMessages.SurnameIsEmpty)
                    .WithErrorCode(UpdateUserValidationErrorCodes.SurnameIsEmpty);
        }
    }
}
