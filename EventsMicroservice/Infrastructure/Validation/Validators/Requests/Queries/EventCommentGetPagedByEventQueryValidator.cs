using Application.MediatR.Queries.EventCommentQueries;
using FluentValidation;
using Shared.Common;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class EventCommentGetPagedByEventQueryValidator : AbstractValidator<EventCommentGetPagedByEventQuery>
    {
        public EventCommentGetPagedByEventQueryValidator(IValidator<PageParameters> pageParametersValidator)
        {
            RuleFor(ec => ec.EventId)
                .NotEmpty();

            RuleFor(ec => ec.PageParameters)
                .SetValidator(pageParametersValidator);
        }
    }
}
