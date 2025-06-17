using Application.Contracts;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators
{
    public class CreateUserTransactionDtoValidator : AbstractValidator<CreateUserTransactionDto>
    {
        public CreateUserTransactionDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.EventId)
                .NotEmpty();

            RuleFor(x => x.EventName)
                .NotNull()
                    .WithMessage(CreateUserTransactionDtoValidationMessages.EventNameIsNull)
                    .WithErrorCode(CreateUserTransactionDtoValidationErrorCodes.EventNameIsNull)
                .NotEmpty()
                    .WithMessage(CreateUserTransactionDtoValidationMessages.EventNameIsEmpty)
                    .WithErrorCode(CreateUserTransactionDtoValidationErrorCodes.EventNameIsEmpty);

            RuleFor(x => x.SeatRow)
                .GreaterThan(0)
                    .WithMessage(CreateUserTransactionDtoValidationMessages.SeatRowIsInvalid)
                    .WithErrorCode(CreateUserTransactionDtoValidationErrorCodes.SeatRowIsInvalid);

            RuleFor(x => x.SeatNumber)
                .GreaterThan(0)
                    .WithMessage(CreateUserTransactionDtoValidationMessages.SeatNumberIsInvalid)
                    .WithErrorCode(CreateUserTransactionDtoValidationErrorCodes.SeatNumberIsInvalid);

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                    .WithMessage(CreateUserTransactionDtoValidationMessages.AmountIsInvalid)
                    .WithErrorCode(CreateUserTransactionDtoValidationErrorCodes.AmountIsInvalid);
        }
    }
}
