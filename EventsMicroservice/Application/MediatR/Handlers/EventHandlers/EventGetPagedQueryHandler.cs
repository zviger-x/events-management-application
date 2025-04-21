using Application.Caching.Constants;
using Application.MediatR.Queries.EventQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Common;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventGetPagedQueryHandler : BaseHandler, IRequestHandler<EventGetPagedQuery, PagedCollection<Event>>
    {
        public EventGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<PagedCollection<Event>> Handle(EventGetPagedQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageParameters.PageNumber;
            var pageSize = request.PageParameters.PageSize;
            var cacheKey = CacheKeys.PagedEvents(pageNumber, pageSize);

            var getPageAsyncFunc = (CancellationToken token) =>
                _unitOfWork.EventRepository.GetPagedAsync(pageNumber, pageSize, token);

            return await _cacheService.GetOrSetAsync(cacheKey, getPageAsyncFunc, cancellationToken);
        }
    }
}
