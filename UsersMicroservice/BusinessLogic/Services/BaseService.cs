using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using DataAccess.Entities.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Services
{
    public abstract class BaseService : IDisposable
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public virtual void Dispose() => _unitOfWork.Dispose();
    }

    public abstract class BaseService<T> : BaseService, IService<T>, IDisposable
        where T : class, IEntity
    {
        protected readonly IValidator<T> _validator;

        protected BaseService(IUnitOfWork unitOfWork, IValidator<T> validator)
            : base(unitOfWork)
        {
            _validator = validator;
        }

        public abstract Task CreateAsync(T entity, CancellationToken token = default);

        public abstract Task UpdateAsync(T entity, CancellationToken token = default);

        public abstract Task DeleteAsync(Guid id, CancellationToken token = default);

        public abstract Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default);

        public abstract Task<PagedCollection<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default);

        public abstract Task<T> GetByIdAsync(Guid id, CancellationToken token = default);

        public virtual async Task SaveChangesAsync(CancellationToken token = default) => await _unitOfWork.SaveChangesAsync(token);
    }
}
