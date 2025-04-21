using Application.Contracts;
using Application.MediatR.Commands.EventCommentCommands;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class EventCommentUpdateCommandValidator : AbstractValidator<EventCommentUpdateCommand>
    {
        public EventCommentUpdateCommandValidator(IValidator<UpdateEventCommentDto> updateEventCommentDtoValidator)
        {
            RuleFor(ec => ec.EventId)
                .NotEmpty();

            RuleFor(ec => ec.CommentId)
                .NotEmpty();

            RuleFor(ec => ec.CurrentUserId)
                .NotEmpty();

            RuleFor(ec => ec.EventComment)
                .SetValidator(updateEventCommentDtoValidator);
        }
    }
}
