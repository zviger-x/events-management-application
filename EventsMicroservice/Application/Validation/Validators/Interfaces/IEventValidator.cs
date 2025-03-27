using Domain.Entities;
using FluentValidation;

namespace Application.Validation.Validators.Interfaces
{
    public interface IEventValidator : IValidator<Event>
    {
    }
}
