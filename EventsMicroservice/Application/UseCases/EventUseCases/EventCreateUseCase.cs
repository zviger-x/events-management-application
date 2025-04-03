using Application.Contracts;
using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.EventUseCases
{
    public class EventCreateUseCase : BaseUseCase<CreateEventDTO>, ICreateUseCaseAsync<CreateEventDTO>
    {
        public EventCreateUseCase(IUnitOfWork unitOfWork, IMapper mapper, ICreateEventDTOValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Execute(CreateEventDTO eventDTO, CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAndThrowAsync(eventDTO);

            var @event = _mapper.Map<Event>(eventDTO);

            await _unitOfWork.EventRepository.CreateAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
