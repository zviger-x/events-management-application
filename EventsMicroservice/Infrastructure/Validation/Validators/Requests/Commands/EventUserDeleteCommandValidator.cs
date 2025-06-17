using Application.MediatR.Commands.EventUserCommands;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class EventUserDeleteCommandValidator : AbstractValidator<EventUserDeleteCommand>
    {
        public EventUserDeleteCommandValidator()
        {
            RuleFor(eu => eu.EventId)
                .NotEmpty();

            RuleFor(eu => eu.UserId)
                .NotEmpty();
        }
    }
}
