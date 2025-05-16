using Application.MediatR.Queries.EventCommentQueries;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class EventCommentGetByIdQueryValidator : AbstractValidator<EventCommentGetByIdQuery>
    {
        public EventCommentGetByIdQueryValidator()
        {
            RuleFor(ec => ec.Id)
                .NotEmpty();
        }
    }
}
