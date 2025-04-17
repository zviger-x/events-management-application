using AutoMapper;
using BusinessLogic.Exceptions;
using BusinessLogic.Services.Interfaces;
using DataAccess.Common;
using DataAccess.Entities.Interfaces;
using DataAccess.UnitOfWork.Interfaces;
using FluentValidation;

namespace BusinessLogic.Services
{
    public abstract class BaseService : IDisposable
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        protected BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public virtual void Dispose() => _unitOfWork.Dispose();
    }

    public abstract class BaseService<T> : BaseService, IService<T>, IDisposable
        where T : class, IEntity
    {
        protected readonly IValidator<T> _validator;

        protected BaseService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<T> validator)
            : base(unitOfWork, mapper)
        {
            _validator = validator;
        }

        public virtual async Task CreateAsync(T entity, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(entity, token);

            await _unitOfWork.Repository<T>().CreateAsync(entity, token);
        }

        public virtual async Task UpdateAsync(Guid routeEntityId, T entity, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(entity, token);

            if (routeEntityId != entity.Id)
                throw new ParameterException("You are not allowed to change this.");

            var storedEntity = await _unitOfWork.Repository<T>().GetByIdAsync(entity.Id, token);
            if (storedEntity == null)
                throw new NotFoundException("Not found.");

            await _unitOfWork.Repository<T>().UpdateAsync(entity, token);
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            var entity = await _unitOfWork.Repository<T>().GetByIdAsync(id, token);
            if (entity == null)
                return;

            await _unitOfWork.Repository<T>().DeleteAsync(entity, token);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
        {
            return await _unitOfWork.Repository<T>().GetAllAsync(token);
        }

        public virtual async Task<PagedCollection<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            if (pageNumber < 1)
                throw new ParameterException(nameof(pageNumber));

            if (pageSize < 1)
                throw new ParameterException(nameof(pageSize));

            return await _unitOfWork.Repository<T>().GetPagedAsync(pageNumber, pageSize, token);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var entity = await _unitOfWork.Repository<T>().GetByIdAsync(id, token);

            return entity ?? throw new NotFoundException("Not found");
        }
    }
}
