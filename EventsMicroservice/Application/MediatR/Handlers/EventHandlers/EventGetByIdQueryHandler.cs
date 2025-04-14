using Application.Caching.Constants;
using Application.MediatR.Queries.EventQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventGetByIdQueryHandler : BaseHandler, IRequestHandler<EventGetByIdQuery, Event>
    {
        public EventGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<Event> Handle(EventGetByIdQuery request, CancellationToken cancellationToken)
        {
            var @event = await _unitOfWork.EventRepository.GetByIdAsync(request.Id, cancellationToken);
            if (@event == null)
                throw new NotFoundException("Event not found.");

            return @event;
        }
    }
}
