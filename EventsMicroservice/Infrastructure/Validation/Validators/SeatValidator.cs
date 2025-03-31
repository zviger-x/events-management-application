using Application.Validation.Validators.Interfaces;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators
{
    public class SeatValidator : AbstractValidator<Seat>, ISeatValidator
    {
        public SeatValidator()
        {
            // TODO: Добавить проверку на наличие ивента
            // TODO: Добавить проверку на уникальность номера в конкретном ряду

            RuleFor(s => s.EventId)
                .NotNull()
                    .WithMessage(SeatValidationMessages.EventIdIsNull)
                    .WithErrorCode(SeatValidationErrorCodes.EventIdIsNull)
                .NotEmpty()
                    .WithMessage(SeatValidationMessages.EventIdIsEmpty)
                    .WithErrorCode(SeatValidationErrorCodes.EventIdIsEmpty);

            RuleFor(s => s.Row)
                .GreaterThan(0)
                    .WithMessage(SeatValidationMessages.RowIsInvalid)
                    .WithErrorCode(SeatValidationErrorCodes.RowIsInvalid);

            RuleFor(s => s.Number)
                .GreaterThan(0)
                    .WithMessage(SeatValidationMessages.NumberIsInvalid)
                    .WithErrorCode(SeatValidationErrorCodes.NumberIsInvalid);

            RuleFor(s => s.Price)
                .GreaterThan(0)
                    .WithMessage(SeatValidationMessages.PriceIsInvalid)
                    .WithErrorCode(SeatValidationErrorCodes.PriceIsInvalid);

            RuleFor(s => s.IsBought)
                .NotNull()
                    .WithMessage(SeatValidationMessages.IsBoughtIsInvalid)
                    .WithErrorCode(SeatValidationErrorCodes.IsBoughtIsInvalid);
        }
    }
}
