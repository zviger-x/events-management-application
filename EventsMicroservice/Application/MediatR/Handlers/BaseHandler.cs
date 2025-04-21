using Application.UnitOfWork.Interfaces;
using AutoMapper;
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
}
