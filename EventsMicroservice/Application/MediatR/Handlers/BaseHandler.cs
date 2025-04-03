using Application.UnitOfWork.Interfaces;
using AutoMapper;
using FluentValidation;

namespace Application.MediatR.Handlers
{
    public abstract class BaseHandler : IDisposable
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public BaseHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public virtual void Dispose() => _unitOfWork.Dispose();
    }

    public abstract class BaseHandler<TValidatingObject> : BaseHandler
        where TValidatingObject : class
    {
        protected readonly IValidator<TValidatingObject> _validator;

        protected BaseHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<TValidatingObject> validator)
            : base(unitOfWork, mapper)
        {
            _validator = validator;
        }
    }
}
