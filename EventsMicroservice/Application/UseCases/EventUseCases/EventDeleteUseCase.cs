using Application.UnitOfWork.Interfaces;
using Application.UseCases.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.EventUseCases
{
    public class EventDeleteUseCase : BaseUseCase<Event>, IDeleteUseCaseAsync<Event>
    {
        public EventDeleteUseCase(IUnitOfWork unitOfWork, IMapper mapper, IEventValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Execute(Guid id, CancellationToken cancellationToken = default)
        {
            var @event = new Event() { Id = id };
            await _unitOfWork.EventRepository.DeleteAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
