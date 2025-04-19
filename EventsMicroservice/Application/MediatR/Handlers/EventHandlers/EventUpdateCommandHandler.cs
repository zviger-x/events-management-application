using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventUpdateCommandHandler : BaseHandler<UpdateEventDto>, IRequestHandler<EventUpdateCommand>
    {
        public EventUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IUpdateEventDtoValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task Handle(EventUpdateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.Event, cancellationToken);

            var storedEvent = await _unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (storedEvent == null)
                throw new NotFoundException("Event not found.");

            var @event = _mapper.Map(request.Event, storedEvent);

            await _unitOfWork.EventRepository.UpdateAsync(@event, cancellationToken);
        }
    }
}
