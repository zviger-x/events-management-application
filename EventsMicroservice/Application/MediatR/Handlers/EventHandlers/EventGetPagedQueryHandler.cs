using Application.MediatR.Queries.EventQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventGetPagedQueryHandler : BaseHandler, IRequestHandler<EventGetPagedQuery, PagedCollection<Event>>
    {
        public EventGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<PagedCollection<Event>> Handle(EventGetPagedQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber< 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageNumber));

            if (request.PageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageSize));

            return await _unitOfWork.EventRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken).ConfigureAwait(false);
        }
    }
}
