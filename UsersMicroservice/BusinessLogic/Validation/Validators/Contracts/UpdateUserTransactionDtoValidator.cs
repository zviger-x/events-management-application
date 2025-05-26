using BusinessLogic.Contracts;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class UpdateUserTransactionDtoValidator : AbstractValidator<UpdateUserTransactionDto>
    {
        public UpdateUserTransactionDtoValidator()
        {
            RuleFor(t => t.Amount)
                .NotNull()
                    .WithMessage(UserTransactionValidationMessages.AmountIsNull)
                    .WithErrorCode(UserTransactionValidationErrorCodes.AmountIsNull)
                .GreaterThanOrEqualTo(0)
                    .WithMessage(UserTransactionValidationMessages.AmountIsLessThanZero)
                    .WithErrorCode(UserTransactionValidationErrorCodes.AmountIsLessThanZero);

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
