using BusinessLogic.Services.Interfaces;
using DataAccess.Entities.Interfaces;
using DataAccess.UnitOfWork.Interfaces;

namespace BusinessLogic.Services
{
    internal abstract class BaseService<T> : IService<T>
        where T : IEntity
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public abstract Task CreateAsync(T entity);

        public abstract Task UpdateAsync(T entity);

        public abstract Task DeleteAsync(T entity);

        public abstract IQueryable<T> GetAll();

        public abstract Task<T> GetByIdAsync(int id);

        public virtual async Task SaveChangesAsync() => await _unitOfWork.SaveChangesAsync();

        public virtual void Dispose() => _unitOfWork.Dispose();
    }
}
