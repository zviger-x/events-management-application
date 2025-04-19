using Application.Contracts;
using Application.Validation.Validators.Interfaces;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators
{
    public class CreateEventCommentDtoValidator : AbstractValidator<CreateEventCommentDto>, ICreateEventCommentDtoValidator
    {
        public CreateEventCommentDtoValidator()
        {
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
        }
    }
}
