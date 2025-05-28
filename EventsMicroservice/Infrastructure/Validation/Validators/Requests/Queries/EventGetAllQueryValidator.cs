using Application.MediatR.Queries.EventQueries;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class EventGetAllQueryValidator : AbstractValidator<EventGetAllQuery>
    {
        public EventGetAllQueryValidator()
        {
        }
    }
}
