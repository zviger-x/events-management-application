using BusinessLogic.Services.Interfaces;
using DataAccess.Entities.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Services
{
    public abstract class BaseService<T> : IService<T>
        where T : class, IEntity
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IValidator<T> _validator;

        protected BaseService(IUnitOfWork unitOfWork, IValidator<T> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public abstract Task CreateAsync(T entity, CancellationToken token = default);

        public abstract Task UpdateAsync(T entity, CancellationToken token = default);

        public abstract Task DeleteAsync(Guid id, CancellationToken token = default);

        public abstract IEnumerable<T> GetAll();

        public abstract Task<T> GetByIdAsync(Guid id, CancellationToken token = default);

        public virtual async Task SaveChangesAsync(CancellationToken token = default) => await _unitOfWork.SaveChangesAsync(token);

        public virtual void Dispose() => _unitOfWork.Dispose();
    }
}
