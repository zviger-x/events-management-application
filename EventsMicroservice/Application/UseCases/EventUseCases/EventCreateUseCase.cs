using Application.Contracts;
using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.EventUseCases
{
    public class EventCreateUseCase : BaseUseCase<EventDTO>, ICreateUseCaseAsync<EventDTO>
    {
        public EventCreateUseCase(IUnitOfWork unitOfWork, IMapper mapper, IEventDTOValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Execute(EventDTO eventDTO, CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAndThrowAsync(eventDTO);

            var @event = _mapper.Map<Event>(eventDTO);

            await _unitOfWork.EventRepository.CreateAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
