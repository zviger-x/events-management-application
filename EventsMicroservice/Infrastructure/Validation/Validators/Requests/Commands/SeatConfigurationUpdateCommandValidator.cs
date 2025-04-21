using Application.Contracts;
using Application.MediatR.Commands.SeatConfigurationCommands;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Commands
{
    public class SeatConfigurationUpdateCommandValidator : AbstractValidator<SeatConfigurationUpdateCommand>
    {
        public SeatConfigurationUpdateCommandValidator(IValidator<UpdateSeatConfigurationDto> updateSeatConfigurationDtoValidator)
        {
            RuleFor(sc => sc.SeatConfigurationId)
                .NotEmpty();

            RuleFor(sc => sc.SeatConfiguration)
                .SetValidator(updateSeatConfigurationDtoValidator);
        }
    }
}
