using BusinessLogic.Contracts;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class ChangePasswordDTOValidator : BaseValidator<ChangePasswordDTO>, IChangePasswordDTOValidator
    {
        public ChangePasswordDTOValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            #warning Сделать проверку на верность текущего пароля

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
