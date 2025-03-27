using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Infrastructure.UseCases.EventUseCases
{
    public class EventCreateUseCase : BaseUseCase<Event>, ICreateUseCaseAsync<Event>
    {
        public EventCreateUseCase(IUnitOfWork unitOfWork, IMapper mapper, IValidator<Event> validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Execute(Event @event, CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAndThrowAsync(@event);

            @event.Id = default;
            await _unitOfWork.EventRepository.CreateAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
