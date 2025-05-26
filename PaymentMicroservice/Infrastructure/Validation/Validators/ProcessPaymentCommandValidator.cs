using Application.MediatR.Commands;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators
{
    public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
    {
        public ProcessPaymentCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.EventId)
                .NotEmpty();

            RuleFor(x => x.EventName)
                .NotNull()
                    .WithMessage(ProcessPaymentCommandValidationMessages.EventNameIsNull)
                    .WithErrorCode(ProcessPaymentCommandValidationErrorCodes.EventNameIsNull)
                .NotEmpty()
                    .WithMessage(ProcessPaymentCommandValidationMessages.EventNameIsEmpty)
                    .WithErrorCode(ProcessPaymentCommandValidationErrorCodes.EventNameIsEmpty);

            RuleFor(x => x.Token)
                .NotNull()
                    .WithMessage(ProcessPaymentCommandValidationMessages.TokenNameIsNull)
                    .WithErrorCode(ProcessPaymentCommandValidationErrorCodes.TokenNameIsNull)
                .NotEmpty()
                    .WithMessage(ProcessPaymentCommandValidationMessages.TokenNameIsEmpty)
                    .WithErrorCode(ProcessPaymentCommandValidationErrorCodes.TokenNameIsEmpty);

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                    .WithMessage(ProcessPaymentCommandValidationMessages.AmountIsInvalid)
                    .WithErrorCode(ProcessPaymentCommandValidationErrorCodes.AmountIsInvalid);

            RuleFor(x => x.SeatRow)
                .GreaterThan(0)
                    .WithMessage(ProcessPaymentCommandValidationMessages.SeatRowIsInvalid)
                    .WithErrorCode(ProcessPaymentCommandValidationErrorCodes.SeatRowIsInvalid);

            RuleFor(x => x.SeatNumber)
                .GreaterThan(0)
                    .WithMessage(ProcessPaymentCommandValidationMessages.SeatNumberIsInvalid)
                    .WithErrorCode(ProcessPaymentCommandValidationErrorCodes.SeatNumberIsInvalid);
        }
    }
}
