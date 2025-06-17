using Application.MediatR.Queries.EventCommentQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Common;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentGetPagedQueryHandler : BaseHandler, IRequestHandler<EventCommentGetPagedQuery, PagedCollection<EventComment>>
    {
        public EventCommentGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<PagedCollection<EventComment>> Handle(EventCommentGetPagedQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EventCommentRepository.GetPagedAsync(
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken);
        }
    }
}
