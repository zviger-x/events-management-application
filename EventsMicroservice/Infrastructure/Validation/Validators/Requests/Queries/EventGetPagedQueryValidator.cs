using Application.MediatR.Queries.EventQueries;
using FluentValidation;
using Shared.Common;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class EventGetPagedQueryValidator : AbstractValidator<EventGetPagedQuery>
    {
        public EventGetPagedQueryValidator(IValidator<PageParameters> pageParametersValidator)
        {
            RuleFor(e => e.PageParameters)
                .SetValidator(pageParametersValidator);
        }
    }
}
