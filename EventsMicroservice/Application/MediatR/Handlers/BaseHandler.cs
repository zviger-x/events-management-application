using Application.UnitOfWork.Interfaces;
using AutoMapper;
using FluentValidation;
using Shared.Caching.Services.Interfaces;

namespace Application.MediatR.Handlers
{
    public abstract class BaseHandler : IDisposable
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ICacheService _cacheService;

        public BaseHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public virtual void Dispose() => _unitOfWork.Dispose();
    }

    public abstract class BaseHandler<TValidatingObject> : BaseHandler
        where TValidatingObject : class
    {
        protected readonly IValidator<TValidatingObject> _validator;

        protected BaseHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IValidator<TValidatingObject> validator)
            : base(unitOfWork, mapper, cacheService)
        {
            _validator = validator;
        }
    }
}
