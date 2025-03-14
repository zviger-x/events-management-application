using DataAccess.Entities.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class BaseValidator<T> : AbstractValidator<T>
        where T : class, IEntity
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
