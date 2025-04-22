using AutoMapper;
using DataAccess.UnitOfWork.Interfaces;

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
}
