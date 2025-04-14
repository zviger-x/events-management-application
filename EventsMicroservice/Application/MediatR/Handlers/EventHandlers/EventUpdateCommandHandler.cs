using Application.Caching.Constants;
using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Interfaces;
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
            
            if (request.RouteEventId != request.Event.Id)
                throw new ParameterException("You are not allowed to modify this event.");

            var @event = _mapper.Map<Event>(request.Event);
            await _unitOfWork.EventRepository.UpdateAsync(@event, cancellationToken);

            await _cacheService.RemoveAsync(CacheKeys.EventById(@event.Id), cancellationToken);
        }
    }
}
