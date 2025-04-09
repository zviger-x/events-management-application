using Application.MediatR.Queries.EventUserQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserGetPagedByEventQueryHandler : BaseHandler, IRequestHandler<EventUserGetPagedByEventQuery, PagedCollection<EventUser>>
    {
        public EventUserGetPagedByEventQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<PagedCollection<EventUser>> Handle(EventUserGetPagedByEventQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageNumber));

            if (request.PageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageSize));

            return await _unitOfWork.EventUserRepository.GetPagedByEventAsync(request.EventId, request.PageNumber, request.PageSize, cancellationToken).ConfigureAwait(false);
        }
    }
}
