using Application.Contracts;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators.Contracts
{
    public class CreateEventUserDtoValidator : AbstractValidator<CreateEventUserDto>
    {
        public CreateEventUserDtoValidator()
        {
            RuleFor(eu => eu.EventId)
                .NotNull()
                    .WithMessage(EventUserValidationMessages.EventIdIsNull)
                    .WithErrorCode(EventUserValidationErrorCodes.EventIdIsNull)
                .NotEmpty()
                    .WithMessage(EventUserValidationMessages.EventIdIsEmpty)
                    .WithErrorCode(EventUserValidationErrorCodes.EventIdIsEmpty);

            RuleFor(eu => eu.UserId)
                .NotNull()
                    .WithMessage(EventUserValidationMessages.UserIdIsNull)
                    .WithErrorCode(EventUserValidationErrorCodes.UserIdIsNull)
                .NotEmpty()
                    .WithMessage(EventUserValidationMessages.UserIdIsEmpty)
                    .WithErrorCode(EventUserValidationErrorCodes.UserIdIsEmpty);

            RuleFor(eu => eu.SeatId)
                .NotNull()
                    .WithMessage(EventUserValidationMessages.SeatIdIsNull)
                    .WithErrorCode(EventUserValidationErrorCodes.SeatIdIsNull)
                .NotEmpty()
                    .WithMessage(EventUserValidationMessages.SeatIdIsEmpty)
                    .WithErrorCode(EventUserValidationErrorCodes.SeatIdIsEmpty);

            RuleFor(eu => eu.Token)
                .NotNull()
                    .WithMessage(EventUserValidationMessages.TokenIsNull)
                    .WithErrorCode(EventUserValidationErrorCodes.TokenIsNull)
                .NotEmpty()
                    .WithMessage(EventUserValidationMessages.TokenIsEmpty)
                    .WithErrorCode(EventUserValidationErrorCodes.TokenIsEmpty);
        }
    }
}
