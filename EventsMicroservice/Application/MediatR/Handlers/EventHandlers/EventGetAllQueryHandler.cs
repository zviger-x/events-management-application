using Application.MediatR.Queries.EventQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventGetAllQueryHandler : BaseHandler, IRequestHandler<EventGetAllQuery, IEnumerable<Event>>
    {
        public EventGetAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<IEnumerable<Event>> Handle(EventGetAllQuery request, CancellationToken cancellationToken)
        {
            var events = await _unitOfWork.EventRepository.GetAllAsync(cancellationToken);

            return events;
        }
    }
}
