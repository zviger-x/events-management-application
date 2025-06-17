using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventUpdateCommandHandler : BaseHandler, IRequestHandler<EventUpdateCommand>
    {
        public EventUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task Handle(EventUpdateCommand request, CancellationToken cancellationToken)
        {
            var storedEvent = await _unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (storedEvent == null)
                throw new NotFoundException("Event not found.");

            var @event = _mapper.Map(request.Event, storedEvent);

            await _unitOfWork.EventRepository.UpdateAsync(@event, cancellationToken);
        }
    }
}
