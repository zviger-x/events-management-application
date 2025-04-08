using Application.Validation.Validators.Interfaces;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators
{
    public class EventCommentValidator : AbstractValidator<EventComment>, IEventCommentValidator
    {
        public EventCommentValidator()
        {
            // TODO: Добавить проверку на наличие ивента
            // TODO: Добавить проверку на наличие пользователя (gRPC)

            RuleFor(r => r.EventId)
                .NotNull()
                    .WithMessage(EventCommentValidationMessages.EventIdIsNull)
                    .WithErrorCode(EventCommentValidationErrorCodes.EventIdIsNull)
                .NotEmpty()
                    .WithMessage(EventCommentValidationMessages.EventIdIsEmpty)
                    .WithErrorCode(EventCommentValidationErrorCodes.EventIdIsEmpty);

            RuleFor(r => r.UserId)
                .NotNull()
                    .WithMessage(EventCommentValidationMessages.UserIdIsNull)
                    .WithErrorCode(EventCommentValidationErrorCodes.UserIdIsNull)
                .NotEmpty()
                    .WithMessage(EventCommentValidationMessages.UserIdIsEmpty)
                    .WithErrorCode(EventCommentValidationErrorCodes.UserIdIsEmpty);

            RuleFor(r => r.Text)
                .NotNull()
                    .WithMessage(EventCommentValidationMessages.TextIsNull)
                    .WithErrorCode(EventCommentValidationErrorCodes.TextIsNull)
                .NotEmpty()
                    .WithMessage(EventCommentValidationMessages.TextIsEmpty)
                    .WithErrorCode(EventCommentValidationErrorCodes.TextIsEmpty)
                .MaximumLength(1000)
                    .WithMessage(EventCommentValidationMessages.TextIsTooLong)
                    .WithErrorCode(EventCommentValidationErrorCodes.TextIsTooLong);

            RuleFor(r => r.CreationTime)
                .GreaterThan(_ => DateTime.MinValue)
                    .WithMessage(EventCommentValidationMessages.CreationTimeIsInvalid)
                    .WithErrorCode(EventCommentValidationErrorCodes.CreationTimeIsInvalid);
        }
    }
}
