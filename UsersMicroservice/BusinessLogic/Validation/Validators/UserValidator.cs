using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class UserValidator : BaseValidator<User>, IUserValidator
    {
        public UserValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
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
                    .WithErrorCode(UserValidationErrorCodes.EmailIsInvalid)
                .MustAsync(IsUniqueEmail)
                    .WithMessage(UserValidationMessages.EmailIsNotUnique)
                    .WithErrorCode(UserValidationErrorCodes.EmailIsNotUnique);
        }

        private async Task<bool> IsUniqueEmail(User user, string email, CancellationToken token)
        {
            var userFromContext = await _unitOfWork.UserRepository.GetByIdAsync(user.Id);

            // Если пользователь не существует, то возвращаем true, потому что email уникален для нового пользователя
            if (userFromContext == null)
                return true;

            // Если email не изменился, то возвращаем true (не нужно проверять уникальность, это тот же email)
            if (userFromContext.Email == user.Email)
                return true;

            // Проверяем, существует ли другой пользователь с нашим новым email
            return !await _unitOfWork.UserRepository.ContainsEmailAsync(email);
        }
    }
}
