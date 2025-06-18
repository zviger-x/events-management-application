using Application.MediatR.Queries.EventUserQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Common;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserGetPagedByEventQueryHandler : BaseHandler, IRequestHandler<EventUserGetPagedByEventQuery, PagedCollection<EventUser>>
    {
        public EventUserGetPagedByEventQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<PagedCollection<EventUser>> Handle(EventUserGetPagedByEventQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EventUserRepository.GetPagedByEventAsync(
                request.EventId,
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken);
        }
    }
}
