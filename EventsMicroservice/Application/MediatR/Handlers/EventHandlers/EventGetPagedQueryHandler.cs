using Application.Caching.Constants;
using Application.MediatR.Queries.EventQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Common;
using Shared.Validation.Interfaces;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventGetPagedQueryHandler : BaseHandler<PageParameters>, IRequestHandler<EventGetPagedQuery, PagedCollection<Event>>
    {
        public EventGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IPageParametersValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task<PagedCollection<Event>> Handle(EventGetPagedQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.PageParameters, cancellationToken);

            var pageNumber = request.PageParameters.PageNumber;
            var pageSize = request.PageParameters.PageSize;
            var cacheKey = CacheKeys.PagedEvents(pageNumber, pageSize);

            var getPageAsyncFunc = (CancellationToken token) =>
                _unitOfWork.EventRepository.GetPagedAsync(pageNumber, pageSize, token);

            return await _cacheService.GetOrSetAsync(cacheKey, getPageAsyncFunc, cancellationToken);
        }
    }
}
