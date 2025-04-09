using Application.MediatR.Queries.EventCommentQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentGetPagedByEventQueryHandler : BaseHandler, IRequestHandler<EventCommentGetPagedByEventQuery, PagedCollection<EventComment>>
    {
        public EventCommentGetPagedByEventQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<PagedCollection<EventComment>> Handle(EventCommentGetPagedByEventQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageNumber));

            if (request.PageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageSize));

            return await _unitOfWork.EventCommentRepository.GetPagedByEventAsync(request.EventId, request.PageNumber, request.PageSize, cancellationToken).ConfigureAwait(false);
        }
    }
}
