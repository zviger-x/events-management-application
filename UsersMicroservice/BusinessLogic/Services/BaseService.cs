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

        public virtual async Task CreateAsync(T entity, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(entity, token);

            entity.Id = default;
            await _unitOfWork.Repository<T>().CreateAsync(entity, token);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(entity, token);

            await _unitOfWork.Repository<T>().UpdateAsync(entity, token);
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            var entity = Activator.CreateInstance<T>();
            entity.Id = id;
            await _unitOfWork.Repository<T>().DeleteAsync(entity, token);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
        {
            return await _unitOfWork.Repository<T>().GetAllAsync(token);
        }

        public virtual async Task<PagedCollection<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _unitOfWork.Repository<T>().GetPagedAsync(pageNumber, pageSize, token);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _unitOfWork.Repository<T>().GetByIdAsync(id, token);
        }

        public virtual async Task SaveChangesAsync(CancellationToken token = default) => await _unitOfWork.SaveChangesAsync(token);
    }
}
