using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class UserTransactionValidator : AbstractValidator<UserTransaction>, IUserTransactionValidator
    {
        public UserTransactionValidator()
        {
            RuleFor(t => t.TransactionDate)
                .NotNull()
                    .WithMessage(UserTransactionValidationMessages.DateIsNull)
                    .WithErrorCode(UserTransactionValidationErrorCodes.DateIsNull);

            RuleFor(t => t.Amount)
                .NotNull()
                    .WithMessage(UserTransactionValidationMessages.AmountIsNull)
                    .WithErrorCode(UserTransactionValidationErrorCodes.AmountIsNull)
                .GreaterThanOrEqualTo(0)
                    .WithMessage(UserTransactionValidationMessages.AmountIsLessThanZero)
                    .WithErrorCode(UserTransactionValidationErrorCodes.AmountIsLessThanZero);

            RuleFor(t => t.UserId)
                .NotNull()
                    .WithMessage(UserTransactionValidationMessages.UserIdIsNull)
                    .WithErrorCode(UserTransactionValidationErrorCodes.UserIdIsNull)
                .NotEmpty()
                    .WithMessage(UserTransactionValidationMessages.UserIdIsEmpty)
                    .WithErrorCode(UserTransactionValidationErrorCodes.UserIdIsEmpty);

            RuleFor(t => t.EventId)
                .NotNull()
                    .WithMessage(UserTransactionValidationMessages.EventIdIsNull)
                    .WithErrorCode(UserTransactionValidationErrorCodes.EventIdIsNull)
                .NotEmpty()
                    .WithMessage(UserTransactionValidationMessages.EventIdIsEmpty)
                    .WithErrorCode(UserTransactionValidationErrorCodes.EventIdIsEmpty);

            RuleFor(t => t.SeatRow)
                .NotNull()
                    .WithMessage(UserTransactionValidationMessages.SeatRowIsNull)
                    .WithErrorCode(UserTransactionValidationErrorCodes.SeatRowIsNull)
                .GreaterThan(0)
                    .WithMessage(UserTransactionValidationMessages.SeatNumberIsLessThanOrEqualToZero)
                    .WithErrorCode(UserTransactionValidationErrorCodes.SeatRowIsInvalid);

            RuleFor(t => t.SeatNumber)
                .NotNull()
                    .WithMessage(UserTransactionValidationMessages.SeatNumberIsNull)
                    .WithErrorCode(UserTransactionValidationErrorCodes.SeatNumberIsNull)
                .GreaterThan(0)
                    .WithMessage(UserTransactionValidationMessages.SeatNumberIsLessThanOrEqualToZero)
                    .WithErrorCode(UserTransactionValidationErrorCodes.SeatNumberIsInvalid);
        }
    }
}
