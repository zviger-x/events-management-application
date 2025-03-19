using BusinessLogic.Contracts;
using FluentValidation;

namespace BusinessLogic.Validation.Validators.Interfaces
{
    public interface ILoginDTOValidator : IValidator<LoginDTO>
    {
    }
}
