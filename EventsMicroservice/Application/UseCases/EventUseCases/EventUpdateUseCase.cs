using Application.Contracts;
using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.EventUseCases
{
    public class EventUpdateUseCase : BaseUseCase<UpdateEventDTO>, IUpdateUseCaseAsync<UpdateEventDTO>
    {
        public EventUpdateUseCase(IUnitOfWork unitOfWork, IMapper mapper, IUpdateEventDTOValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Execute(UpdateEventDTO eventDTO, CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAndThrowAsync(eventDTO);

            var @event = _mapper.Map<Event>(eventDTO);

            await _unitOfWork.EventRepository.UpdateAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
