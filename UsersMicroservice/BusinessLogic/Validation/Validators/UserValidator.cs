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
            var isUnique = !_unitOfWork.UserRepository.GetAll().Any(e => e.Email == email);
            return await Task.FromResult(isUnique);
        }
    }
}
