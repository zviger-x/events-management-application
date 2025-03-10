using BusinessLogic.Validation.Messages;
using DataAccess.Entities;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    internal class UserTransactionValidator : BaseValidator<UserTransaction>
    {
        public UserTransactionValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            RuleFor(t => t.TransactionDate)
                .NotNull().WithMessage(UserTransactionValidationMessages.DateNotNull);

            RuleFor(t => t.Amount)
                .NotNull().WithMessage(UserTransactionValidationMessages.AmountNotNull)
                .GreaterThanOrEqualTo(0).WithMessage(UserTransactionValidationMessages.AmountLessThanZero);

            // Нужно в будущем добавить проверку на наличие
            RuleFor(t => t.UsertId)
                .NotNull().WithMessage(UserTransactionValidationMessages.UserIdNotNull)
                .GreaterThanOrEqualTo(0).WithMessage(UserTransactionValidationMessages.UserIdLessThanZero);

            RuleFor(t => t.EventId)
                .NotNull().WithMessage(UserTransactionValidationMessages.EventIdNotNull)
                .GreaterThanOrEqualTo(0).WithMessage(UserTransactionValidationMessages.EventIdLessThanZero);

            RuleFor(t => t.SeatRow)
                .NotNull().WithMessage(UserTransactionValidationMessages.SeatRowNotNull)
                .GreaterThan(0).WithMessage(UserTransactionValidationMessages.SeatRowLessThanOrEqualsToZero);

            RuleFor(t => t.SeatNumber)
                .NotNull().WithMessage(UserTransactionValidationMessages.SeatNumberNotNull)
                .GreaterThan(0).WithMessage(UserTransactionValidationMessages.SeatNumberLessThanOrEqualsToZero);
        }
    }
}
