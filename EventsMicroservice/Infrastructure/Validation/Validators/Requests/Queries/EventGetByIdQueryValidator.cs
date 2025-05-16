using Application.MediatR.Queries.EventQueries;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class EventGetByIdQueryValidator : AbstractValidator<EventGetByIdQuery>
    {
        public EventGetByIdQueryValidator()
        {
            RuleFor(e => e.Id)
                .NotEmpty();
        }
    }
}
