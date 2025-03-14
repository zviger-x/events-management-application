using BusinessLogic.Models;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Validation.Validators.Interfaces;
using DataAccess.Entities.Interfaces;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    public abstract class BaseService<T> : IService<T>
        where T : class, IEntity
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IBaseValidator<T> _validator;

        protected BaseService(IUnitOfWork unitOfWork, IBaseValidator<T> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public abstract Task<Response> CreateAsync(T entity, CancellationToken token = default);

        public abstract Task<Response> UpdateAsync(T entity, CancellationToken token = default);

        public abstract Task<Response> DeleteAsync(Guid id, CancellationToken token = default);

        public abstract Response<IEnumerable<T>> GetAll();

        public abstract Task<Response<T>> GetByIdAsync(Guid id, CancellationToken token = default);

        public virtual async Task SaveChangesAsync(CancellationToken token = default) => await _unitOfWork.SaveChangesAsync(token);

        public virtual void Dispose() => _unitOfWork.Dispose();
    }
}
