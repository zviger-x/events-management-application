using BusinessLogic.Contracts;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class LoginDTOValidator : BaseValidator<LoginDTO>, ILoginDTOValidator
    {
        public LoginDTOValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            RuleFor(u => u.Email)
                .NotNull()
                    .WithMessage(LoginDTOValidationMessages.EmailIsNull)
                    .WithErrorCode(LoginDTOValidationErrorCodes.EmailIsNull)
                .NotEmpty()
                    .WithMessage(LoginDTOValidationMessages.EmailIsEmpty)
                    .WithErrorCode(LoginDTOValidationErrorCodes.EmailIsEmpty)
                .EmailAddress()
                    .WithMessage(LoginDTOValidationMessages.EmailIsInvalid)
                    .WithErrorCode(LoginDTOValidationErrorCodes.EmailIsInvalid);

            RuleFor(u => u.Password)
                .NotNull()
                    .WithMessage(LoginDTOValidationMessages.PasswordIsNull)
                    .WithErrorCode(LoginDTOValidationErrorCodes.PasswordIsNull)
                .NotEmpty()
                    .WithMessage(LoginDTOValidationMessages.PasswordIsEmpty)
                    .WithErrorCode(LoginDTOValidationErrorCodes.PasswordIsEmpty);
        }
    }
}
