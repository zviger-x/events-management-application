using Application.MediatR.Queries.SeatConfigurationQueries;
using FluentValidation;
using Shared.Common;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class SeatConfigurationGetPagedQueryValidator : AbstractValidator<SeatConfigurationGetPagedQuery>
    {
        public SeatConfigurationGetPagedQueryValidator(IValidator<PageParameters> pageParametersValidator)
        {
            RuleFor(sc => sc.PageParameters)
                .SetValidator(pageParametersValidator);
        }
    }
}
