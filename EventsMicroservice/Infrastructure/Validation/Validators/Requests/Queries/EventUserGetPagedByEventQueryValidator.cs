using Application.MediatR.Queries.EventUserQueries;
using FluentValidation;
using Shared.Common;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class EventUserGetPagedByEventQueryValidator : AbstractValidator<EventUserGetPagedByEventQuery>
    {
        public EventUserGetPagedByEventQueryValidator(IValidator<PageParameters> pageParametersValidator)
        {
            RuleFor(eu => eu.EventId)
                .NotEmpty();

            RuleFor(eu => eu.PageParameters)
                .SetValidator(pageParametersValidator);
        }
    }
}
