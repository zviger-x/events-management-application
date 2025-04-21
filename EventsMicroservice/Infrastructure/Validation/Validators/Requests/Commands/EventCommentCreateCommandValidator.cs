using Application.Contracts;
using Application.MediatR.Commands.EventCommentCommands;
using FluentValidation;
using Infrastructure.Validation.ErrorCodes;
using Infrastructure.Validation.Messages;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class EventCommentCreateCommandValidator : AbstractValidator<EventCommentCreateCommand>
    {
        public EventCommentCreateCommandValidator(IValidator<CreateEventCommentDto> createEventCommentDtoValidator)
        {
            RuleFor(ec => ec.EventId)
                .NotEmpty();

            RuleFor(ec => ec.EventComment)
                .SetValidator(createEventCommentDtoValidator);
        }
    }
}
