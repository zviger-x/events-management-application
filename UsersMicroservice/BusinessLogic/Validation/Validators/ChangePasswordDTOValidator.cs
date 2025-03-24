using BusinessLogic.Contracts;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class ChangePasswordDTOValidator : BaseValidator<ChangePasswordDTO>, IChangePasswordDTOValidator
    {
        private readonly IPasswordHashingService _passwordHashingService;

        public ChangePasswordDTOValidator(IUnitOfWork unitOfWork, IPasswordHashingService passwordHashingService)
            : base(unitOfWork)
        {
            _passwordHashingService = passwordHashingService;

            RuleFor(u => u.CurrentPassword)
                .NotNull()
                    .WithMessage(ChangePasswordValidationMessages.CurrentPasswordIsNull)
                    .WithErrorCode(ChangePasswordValidationErrorCodes.CurrentPasswordIsNull)
                .NotEmpty()
                    .WithMessage(ChangePasswordValidationMessages.CurrentPasswordIsEmpty)
                    .WithErrorCode(ChangePasswordValidationErrorCodes.CurrentPasswordIsEmpty)
                .MustAsync(IsCurrentPassword)
                    .WithMessage(ChangePasswordValidationMessages.CurrentPasswordIsInvalid)
                    .WithErrorCode(ChangePasswordValidationErrorCodes.CurrentPasswordIsInvalid);

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

        private async Task<bool> IsCurrentPassword(ChangePasswordDTO dto, string currentPassword, CancellationToken token)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(dto.Id);

            if (user == null || !_passwordHashingService.VerifyPassword(currentPassword, user.PasswordHash))
                return false;

            return true;
        }
    }
}
