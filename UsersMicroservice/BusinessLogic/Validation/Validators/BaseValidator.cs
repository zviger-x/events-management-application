using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Validation.Validators
{
    internal class BaseValidator<T> : AbstractValidator<T>, IBaseValidator<T>
        where T : IEntity
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Dictionary<string, string[]>> ValidateAndThrowAsync(T entity)
        {
            var result = await ValidateAsync(entity);

            if (result.IsValid)
                return new();

            // В будущем возможно сделаю так, чтобы на фронтенд приходило в формате error.propertyname.errorcode
            // Чтобы можно было под разные языки подстраивать ошибки
            // А пока для демонстрации так.
            return result.Errors
                .GroupBy(e => e.PropertyName.ToLower())
                .ToDictionary(
                    g => $"error.{g.Key}",
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
        }
    }
}
