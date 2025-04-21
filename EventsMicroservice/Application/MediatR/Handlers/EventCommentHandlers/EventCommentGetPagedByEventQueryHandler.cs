using Application.MediatR.Queries.EventCommentQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Common;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentGetPagedByEventQueryHandler : BaseHandler, IRequestHandler<EventCommentGetPagedByEventQuery, PagedCollection<EventComment>>
    {
        public EventCommentGetPagedByEventQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<PagedCollection<EventComment>> Handle(EventCommentGetPagedByEventQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EventCommentRepository.GetPagedByEventAsync(
                request.EventId,
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken);
        }
    }
}
