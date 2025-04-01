using Application.Contracts;
using FluentValidation;

namespace Application.Validation.Validators.Interfaces
{
    public interface IEventDTOValidator : IValidator<EventDTO>
    {
    }
}
