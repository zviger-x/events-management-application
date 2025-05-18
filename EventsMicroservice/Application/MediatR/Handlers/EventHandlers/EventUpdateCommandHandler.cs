using Application.MediatR.Commands.EventCommands;
using Application.Messages;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;
using Shared.Kafka.Contracts.Events;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventUpdateCommandHandler : BaseHandler, IRequestHandler<EventUpdateCommand>
    {
        private readonly IEventUpdatedMessageProducer _messageProducer;

        public EventUpdateCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            IEventUpdatedMessageProducer eventUpdatedMessageProducer)
            : base(unitOfWork, mapper, cacheService)
        {
            _messageProducer = eventUpdatedMessageProducer;
        }

        public async Task Handle(EventUpdateCommand request, CancellationToken cancellationToken)
        {
            var storedEvent = await _unitOfWork.EventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (storedEvent == null)
                throw new NotFoundException("Event not found.");

            var oldName = new string(storedEvent.Name);
            var @event = _mapper.Map(request.Event, storedEvent);

            await _unitOfWork.EventRepository.UpdateAsync(@event, cancellationToken);

            var eventUsers = await _unitOfWork.EventUserRepository.GetByEventAsync(request.EventId, cancellationToken);
            var eventUsersIds = eventUsers.Select(eu => eu.UserId);

            var message = _mapper.Map<EventUpdatedDto>(request);
            message.Name = oldName;
            message.TargetUsers = eventUsersIds;

            await _messageProducer.PublishAsync(message, cancellationToken);
        }
    }
}
