using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Infrastructure.UseCases.EventUseCases
{
    public class EventGetByIdUseCase : BaseUseCase<Event>, IGetByIdUseCaseAsync<Event>
    {
        public EventGetByIdUseCase(IUnitOfWork unitOfWork, IMapper mapper, IValidator<Event> validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<Event> Execute(Guid id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.EventRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        }
    }
}
