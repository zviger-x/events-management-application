using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class EventUpdateCommandValidator : AbstractValidator<EventUpdateCommand>
    {
        public EventUpdateCommandValidator(IValidator<UpdateEventDto> updateEventDtoValidator)
        {
            RuleFor(e => e.EventId)
                .NotEmpty();

            RuleFor(e => e.Event)
                .SetValidator(updateEventDtoValidator);
        }
    }
}
