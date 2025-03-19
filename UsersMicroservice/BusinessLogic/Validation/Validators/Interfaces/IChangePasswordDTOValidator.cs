using BusinessLogic.Contracts;
using FluentValidation;

namespace BusinessLogic.Validation.Validators.Interfaces
{
    public interface IChangePasswordDTOValidator : IValidator<ChangePasswordDTO>
    {
    }
}
