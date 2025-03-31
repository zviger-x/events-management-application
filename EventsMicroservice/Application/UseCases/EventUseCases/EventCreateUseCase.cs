using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.EventUseCases
{
    public class EventCreateUseCase : BaseUseCase<Event>, ICreateUseCaseAsync<Event>
    {
        public EventCreateUseCase(IUnitOfWork unitOfWork, IMapper mapper, IEventValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Execute(Event @event, CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAndThrowAsync(@event);

            @event.Seats = default!;
            @event.Reviews = default!;

            await _unitOfWork.EventRepository.CreateAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
