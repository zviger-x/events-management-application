using Application.MediatR.Commands.SeatConfigurationCommands;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class SeatConfigurationDeleteCommandValidator : AbstractValidator<SeatConfigurationDeleteCommand>
    {
        public SeatConfigurationDeleteCommandValidator()
        {
            RuleFor(sc => sc.Id)
                .NotEmpty();
        }
    }
}
