using Application.MediatR.Queries.EventUserQueries;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class EventUserGetAllByEventQueryValidator : AbstractValidator<EventUserGetAllByEventQuery>
    {
        public EventUserGetAllByEventQueryValidator()
        {
            RuleFor(eu => eu.EventId)
                .NotEmpty();
        }
    }
}
