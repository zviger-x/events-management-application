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
                    .WithMessage(ChangePasswordDTOValidationMessages.PasswordIsNull)
                    .WithErrorCode(ChangePasswordDTOValidationErrorCodes.PasswordIsNull)
                .NotEmpty()
                    .WithMessage(ChangePasswordDTOValidationMessages.PasswordIsEmpty)
                    .WithErrorCode(ChangePasswordDTOValidationErrorCodes.PasswordIsEmpty);

            RuleFor(u => u.ConfirmPassword)
                .Equal(u => u.NewPassword)
                    .WithMessage(ChangePasswordDTOValidationMessages.PasswordsDoNotMatch)
                    .WithErrorCode(ChangePasswordDTOValidationErrorCodes.PasswordsDoNotMatch);
        }
    }
}
