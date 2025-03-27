using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.UseCases.EventUseCases
{
    public class EventGetAllUseCase : BaseUseCase<Event>, IGetAllUseCaseAsync<Event>
    {
        public EventGetAllUseCase(IUnitOfWork unitOfWork, IMapper mapper, IEventValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<IEnumerable<Event>> Execute(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.EventRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
