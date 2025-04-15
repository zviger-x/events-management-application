using Application.Contracts;
using FluentValidation;

namespace Application.Validation.Validators.Interfaces
{
    public interface ICreateEventUserDtoValidator : IValidator<CreateEventUserDto>
    {
    }
}
