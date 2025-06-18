using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class EventCreateCommandValidator : AbstractValidator<EventCreateCommand>
    {
        public EventCreateCommandValidator(IValidator<CreateEventDto> createEventDtoValidator)
        {
            RuleFor(e => e.Event)
                .SetValidator(createEventDtoValidator);
        }
    }
}
