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
            var name = request.Name;
            var description = request.Description;
            var location = request.Location;
            var fromDate = request.FromDate;
            var toDate = request.ToDate;

            var isFilterEmpty = string.IsNullOrEmpty(name) &&
                string.IsNullOrEmpty(description) &&
                string.IsNullOrEmpty(location) &&
                !fromDate.HasValue &&
                !toDate.HasValue;

            var pageNumber = request.PageParameters.PageNumber;
            var pageSize = request.PageParameters.PageSize;

            // Если фильтр есть, возвращаем по филтру и не кэшируем
            if (!isFilterEmpty)
                return await _unitOfWork.EventRepository.GetPagedByFilterAsync(name, description, location, fromDate, toDate, pageNumber, pageSize, cancellationToken);

            // Если фильтра нет, возвращаем стандартную страницу и кэшируем
            var cacheKey = CacheKeys.PagedEvents(pageNumber, pageSize);

            var getPageAsyncFunc = (CancellationToken token) =>
                _unitOfWork.EventRepository.GetPagedAsync(pageNumber, pageSize, token);

            return await _cacheService.GetOrSetAsync(cacheKey, getPageAsyncFunc, cancellationToken);
        }
    }
}
