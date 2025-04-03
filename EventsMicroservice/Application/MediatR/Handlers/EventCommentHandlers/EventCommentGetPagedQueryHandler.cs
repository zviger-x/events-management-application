using Application.MediatR.Queries.EventCommentQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentGetPagedQueryHandler : BaseHandler, IRequestHandler<EventCommentGetPagedQuery, PagedCollection<EventComment>>
    {
        public EventCommentGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<PagedCollection<EventComment>> Handle(EventCommentGetPagedQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber< 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageNumber));

            if (request.PageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageSize));

            return await _unitOfWork.EventCommentRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken).ConfigureAwait(false);
        }
    }
}
