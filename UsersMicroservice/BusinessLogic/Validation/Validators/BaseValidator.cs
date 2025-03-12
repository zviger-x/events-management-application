using BusinessLogic.Validation.Results;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    public class BaseValidator<T> : AbstractValidator<T>, IBaseValidator<T>
        where T : IEntity
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ValidationResultDictionary> ValidateAndThrowAsync(T entity)
        {
            var result = await ValidateAsync(entity);

            if (result.IsValid)
                return new();

            // В будущем сделаю так, чтобы на фронтенд приходило в формате error.propertyname.errorcode
            // Чтобы можно было под разные языки подстраивать ошибки
            // Сейчас для демонстрации ошибки будут возвращаться текстом
            return new ValidationResultDictionary(result);
        }
    }
}
