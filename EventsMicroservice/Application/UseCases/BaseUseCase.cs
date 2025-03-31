using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities.Interfaces;
using FluentValidation;

namespace Application.UseCases
{
    public abstract class BaseUseCase : IDisposable
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public BaseUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public virtual void Dispose() => _unitOfWork.Dispose();
    }

    public abstract class BaseUseCase<T> : BaseUseCase
        where T : class, IEntity
    {
        protected readonly IValidator<T> _validator;

        protected BaseUseCase(IUnitOfWork unitOfWork, IMapper mapper, IValidator<T> validator)
            : base(unitOfWork, mapper)
        {
            _validator = validator;
        }
    }
}
