using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T>
        where T : class
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
