using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Infrastructure.UseCases.EventUseCases
{
    public class EventGetAllUseCase : BaseUseCase<Event>, IGetAllUseCaseAsync<Event>
    {
        public EventGetAllUseCase(IUnitOfWork unitOfWork, IMapper mapper, IValidator<Event> validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<IEnumerable<Event>> Execute(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.EventRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
