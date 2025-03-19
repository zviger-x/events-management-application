using BusinessLogic.Contracts;
using BusinessLogic.Validation.ErrorCodes;
using BusinessLogic.Validation.Messages;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class UpdateUserDTOValidator : BaseValidator<UpdateUserDTO>, IUpdateUserDTOValidator
    {
        public UpdateUserDTOValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            RuleFor(u => u.Name)
                .NotNull()
                    .WithMessage(UpdateUserDTOValidationMessages.NameIsNull)
                    .WithErrorCode(UpdateUserDTOValidationErrorCodes.NameIsNull)
                .NotEmpty()
                    .WithMessage(UpdateUserDTOValidationMessages.NameIsEmpty)
                    .WithErrorCode(UpdateUserDTOValidationErrorCodes.NameIsEmpty);

            RuleFor(u => u.Surname)
                .NotNull()
                    .WithMessage(UpdateUserDTOValidationMessages.SurnameIsNull)
                    .WithErrorCode(UpdateUserDTOValidationErrorCodes.SurnameIsNull)
                .NotEmpty()
                    .WithMessage(UpdateUserDTOValidationMessages.SurnameIsEmpty)
                    .WithErrorCode(UpdateUserDTOValidationErrorCodes.SurnameIsEmpty);
        }
    }
}
