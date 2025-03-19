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
            #warning Проверить наличие пользователя
            RuleFor(u => u.Email)
                .NotNull()
                    .WithMessage(LoginValidationMessages.EmailIsNull)
                    .WithErrorCode(LoginValidationErrorCodes.EmailIsNull)
                .NotEmpty()
                    .WithMessage(LoginValidationMessages.EmailIsEmpty)
                    .WithErrorCode(LoginValidationErrorCodes.EmailIsEmpty)
                .EmailAddress()
                    .WithMessage(LoginValidationMessages.EmailIsInvalid)
                    .WithErrorCode(LoginValidationErrorCodes.EmailIsInvalid);

            RuleFor(u => u.Password)
                .NotNull()
                    .WithMessage(LoginValidationMessages.PasswordIsNull)
                    .WithErrorCode(LoginValidationErrorCodes.PasswordIsNull)
                .NotEmpty()
                    .WithMessage(LoginValidationMessages.PasswordIsEmpty)
                    .WithErrorCode(LoginValidationErrorCodes.PasswordIsEmpty);
        }
    }
}
