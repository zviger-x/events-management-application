using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Infrastructure.UseCases.EventUseCases
{
    public class EventUpdateUseCase : BaseUseCase<Event>, IUpdateUseCaseAsync<Event>
    {
        public EventUpdateUseCase(IUnitOfWork unitOfWork, IMapper mapper, IValidator<Event> validator)
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
