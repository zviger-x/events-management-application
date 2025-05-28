using Application.MediatR.Queries.SeatQueries;
using FluentValidation;

namespace Infrastructure.Validation.Validators.Requests.Queries
{
    public class SeatGetByEventQueryValidator : AbstractValidator<SeatGetByEventQuery>
    {
        public SeatGetByEventQueryValidator()
        {
            RuleFor(s => s.EventId)
                .NotEmpty();
        }
    }
}
