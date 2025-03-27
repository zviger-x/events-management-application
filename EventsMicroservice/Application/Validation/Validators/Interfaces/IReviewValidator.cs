using Domain.Entities;
using FluentValidation;

namespace Application.Validation.Validators.Interfaces
{
    public interface IReviewValidator : IValidator<Review>
    {
    }
}
