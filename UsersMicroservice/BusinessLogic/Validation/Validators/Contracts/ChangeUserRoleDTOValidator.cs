using BusinessLogic.Contracts;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class ChangeUserRoleDTOValidator : AbstractValidator<ChangeUserRoleDTO>
    {
        public ChangeUserRoleDTOValidator()
        {
        }
    }
}
