using Application.MediatR.Queries.EventUserQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserGetAllByEventQueryHandler : BaseHandler, IRequestHandler<EventUserGetAllByEventQuery, IEnumerable<EventUser>>
    {
        public EventUserGetAllByEventQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<IEnumerable<EventUser>> Handle(EventUserGetAllByEventQuery request, CancellationToken cancellationToken)
        {
            var events = await _unitOfWork.EventUserRepository.GetByEventAsync(request.EventId, cancellationToken);

            return events;
        }
    }
}
