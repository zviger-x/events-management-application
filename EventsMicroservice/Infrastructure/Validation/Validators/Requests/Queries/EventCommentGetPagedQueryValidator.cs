using Application.MediatR.Queries.EventCommentQueries;
using FluentValidation;
using Shared.Common;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class EventCommentGetPagedQueryValidator : AbstractValidator<EventCommentGetPagedQuery>
    {
        public EventCommentGetPagedQueryValidator(IValidator<PageParameters> pageParametersValidator)
        {
            RuleFor(ec => ec.PageParameters)
                .SetValidator(pageParametersValidator);
        }
    }
}
