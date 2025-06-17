using Application.MediatR.Commands.EventCommands;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class EventDeleteCommandValidator : AbstractValidator<EventDeleteCommand>
    {
        public EventDeleteCommandValidator()
        {
            RuleFor(e => e.Id)
                .NotEmpty();
        }
    }
}
