using Application.Contracts;
using Application.MediatR.Commands.EventUserCommands;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class EventUserCreateCommandValidator : AbstractValidator<EventUserCreateCommand>
    {
        public EventUserCreateCommandValidator(IValidator<CreateEventUserDto> createEventUserDtoValidator)
        {
            RuleFor(eu => eu.EventUser)
                .SetValidator(createEventUserDtoValidator);
        }
    }
}
