using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.EventUseCases
{
    public class EventUpdateUseCase : BaseUseCase<Event>, IUpdateUseCaseAsync<Event>
    {
        public EventUpdateUseCase(IUnitOfWork unitOfWork, IMapper mapper, IEventValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Execute(Event @event, CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAndThrowAsync(@event);
            await _unitOfWork.EventRepository.UpdateAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
