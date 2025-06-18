using Application.MediatR.Queries.SeatConfigurationQueries;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class SeatConfigurationGetByIdQueryValidator : AbstractValidator<SeatConfigurationGetByIdQuery>
    {
        public SeatConfigurationGetByIdQueryValidator()
        {
            RuleFor(sc => sc.Id)
                .NotEmpty();
        }
    }
}
