using BusinessLogic.Contracts;
using BusinessLogic.Validation.Validators.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class ChangeUserRoleDTOValidator : AbstractValidator<ChangeUserRoleDTO>, IChangeUserRoleDTOValidator
    {
        public ChangeUserRoleDTOValidator()
        {
        }
    }
}
