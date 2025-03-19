using BusinessLogic.Contracts;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class RegisterDTOValidator : BaseValidator<RegisterDTO>, IRegisterDTOValidator
    {
        public RegisterDTOValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            RuleFor(u => u.Name)
                .NotNull()
                    .WithMessage(RegisterDTOValidationMessages.NameIsNull)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.NameIsNull)
                .NotEmpty()
                    .WithMessage(RegisterDTOValidationMessages.NameIsEmpty)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.NameIsEmpty);

            RuleFor(u => u.Surname)
                .NotNull()
                    .WithMessage(RegisterDTOValidationMessages.SurnameIsNull)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.SurnameIsNull)
                .NotEmpty()
                    .WithMessage(RegisterDTOValidationMessages.SurnameIsEmpty)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.SurnameIsEmpty);

            RuleFor(u => u.Email)
                .NotNull()
                    .WithMessage(RegisterDTOValidationMessages.EmailIsNull)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.EmailIsNull)
                .NotEmpty()
                    .WithMessage(RegisterDTOValidationMessages.EmailIsEmpty)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.EmailIsEmpty)
                .EmailAddress()
                    .WithMessage(RegisterDTOValidationMessages.EmailIsInvalid)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.EmailIsInvalid)
                .MustAsync(IsUniqueEmail)
                    .WithMessage(RegisterDTOValidationMessages.EmailIsNotUnique)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.EmailIsNotUnique);

            RuleFor(u => u.Password)
                .NotNull()
                    .WithMessage(RegisterDTOValidationMessages.PasswordIsNull)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.PasswordIsNull)
                .NotEmpty()
                    .WithMessage(RegisterDTOValidationMessages.PasswordIsEmpty)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.PasswordIsEmpty);

            RuleFor(u => u.ConfirmPassword)
                .Equal(u => u.Password)
                    .WithMessage(RegisterDTOValidationMessages.PasswordsDoNotMatch)
                    .WithErrorCode(RegisterDTOValidationErrorCodes.PasswordsDoNotMatch);
        }

        private async Task<bool> IsUniqueEmail(string email, CancellationToken token)
        {
            return !await _unitOfWork.UserRepository.ContainsEmailAsync(email);
        }
    }
}
