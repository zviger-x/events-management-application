using BusinessLogic.Contracts;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class ChangeUserRoleDtoValidator : AbstractValidator<ChangeUserRoleDto>
    {
        public ChangeUserRoleDtoValidator()
        {
        }
    }
}
