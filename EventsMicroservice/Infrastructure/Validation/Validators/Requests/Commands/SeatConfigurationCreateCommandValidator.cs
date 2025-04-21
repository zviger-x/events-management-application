using Application.Contracts;
using Application.MediatR.Commands.SeatConfigurationCommands;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class SeatConfigurationCreateCommandValidator : AbstractValidator<SeatConfigurationCreateCommand>
    {
        public SeatConfigurationCreateCommandValidator(IValidator<CreateSeatConfigurationDto> createSeatConfigurationDtoValidator)
        {
            RuleFor(sc => sc.SeatConfiguration)
                .SetValidator(createSeatConfigurationDtoValidator);
        }
    }
}
