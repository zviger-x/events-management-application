using Application.MediatR.Queries.EventUserQueries;
using FluentValidation;
using Shared.Common;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class EventUserGetPagedQueryValidator : AbstractValidator<EventUserGetPagedQuery>
    {
        public EventUserGetPagedQueryValidator(IValidator<PageParameters> pageParametersValidator)
        {
            RuleFor(eu => eu.PageParameters)
                .SetValidator(pageParametersValidator);
        }
    }
}
