using BusinessLogic.Validation.Messages;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Validation.Validators
{
    internal class UserValidator : BaseValidator<User>
    {
        public UserValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            RuleFor(u => u.Name)
                .NotNull().WithMessage(UserValidationMessages.NameNotNull)
                .NotEmpty().WithMessage(UserValidationMessages.NameNotEmpty);

            RuleFor(u => u.Surname)
                .NotNull().WithMessage(UserValidationMessages.SurnameNotNull)
                .NotEmpty().WithMessage(UserValidationMessages.SurnameNotEmpty);

            RuleFor(u => u.Email)
                .NotNull().WithMessage(UserValidationMessages.EmailNotNull)
                .NotEmpty().WithMessage(UserValidationMessages.EmailNotEmpty)
                .EmailAddress().WithMessage(UserValidationMessages.EmailInvalid)
                .MustAsync(IsUniqueEmail).WithMessage(UserValidationMessages.EmailMustBeUnique);
        }

        private async Task<bool> IsUniqueEmail(User user, string email, CancellationToken token)
        {
            return !await _unitOfWork.UserRepository.GetAll()
                .AnyAsync(e => e.Email == email, token);
        }
    }
}
