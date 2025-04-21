using Application.MediatR.Commands.EventCommentCommands;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class EventCommentDeleteCommandValidator : AbstractValidator<EventCommentDeleteCommand>
    {
        public EventCommentDeleteCommandValidator()
        {
            RuleFor(ec => ec.EventId)
                .NotEmpty();

            RuleFor(ec => ec.CommentId)
                .NotEmpty();

            RuleFor(ec => ec.CurrentUserId)
                .NotEmpty();
        }
    }
}
