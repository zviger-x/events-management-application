using Application.Contracts;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators
{
    public class CreateSeatConfigurationDtoValidator : AbstractValidator<CreateSeatConfigurationDto>
    {
        public CreateSeatConfigurationDtoValidator()
        {
            RuleFor(sc => sc.Name)
                .NotNull()
                    .WithMessage(SeatConfigurationValidationMessages.NameIsNull)
                    .WithErrorCode(SeatConfigurationValidationErrorCodes.NameIsNull)
                .NotEmpty()
                    .WithMessage(SeatConfigurationValidationMessages.NameIsEmpty)
                    .WithErrorCode(SeatConfigurationValidationErrorCodes.NameIsEmpty);

            RuleFor(sc => sc.DefaultPrice)
                .GreaterThan(0)
                    .WithMessage(SeatConfigurationValidationMessages.DefaultPriceIsInvalid)
                    .WithErrorCode(SeatConfigurationValidationErrorCodes.DefaultPriceIsInvalid);

            RuleFor(sc => sc.Rows)
                .NotNull()
                    .WithMessage(SeatConfigurationValidationMessages.RowsIsNull)
                    .WithErrorCode(SeatConfigurationValidationErrorCodes.RowsIsNull)
                .NotEmpty()
                    .WithMessage(SeatConfigurationValidationMessages.RowsIsEmpty)
                    .WithErrorCode(SeatConfigurationValidationErrorCodes.RowsIsEmpty)
                .Must(rows => rows.All(row => row > 0))  // Проверка, что все значения в Rows положительные
                    .WithMessage(SeatConfigurationValidationMessages.RowsAreInvalid)
                    .WithErrorCode(SeatConfigurationValidationErrorCodes.RowsAreInvalid);
        }
    }
}
