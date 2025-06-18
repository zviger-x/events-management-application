using Application.MediatR.Queries.EventUserQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Common;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserGetPagedQueryHandler : BaseHandler, IRequestHandler<EventUserGetPagedQuery, PagedCollection<EventUser>>
    {
        public EventUserGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<PagedCollection<EventUser>> Handle(EventUserGetPagedQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EventUserRepository.GetPagedAsync(
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken);
        }
    }
}
