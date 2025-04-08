using Application.Validation.Validators.Interfaces;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators
{
    public class EventUserValidator : AbstractValidator<EventUser>, IEventUserValidator
    {
        public EventUserValidator()
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

            RuleFor(eu => eu.RegisteredAt)
                .LessThanOrEqualTo(DateTime.UtcNow)
                    .WithMessage(EventUserValidationMessages.RegistrationTimeIsInvalid)
                    .WithErrorCode(EventUserValidationErrorCodes.RegistrationTimeIsInvalid);
        }
    }
}
