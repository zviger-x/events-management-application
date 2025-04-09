using Application.MediatR.Queries.EventUserQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserGetPagedQueryHandler : BaseHandler, IRequestHandler<EventUserGetPagedQuery, PagedCollection<EventUser>>
    {
        public EventUserGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<PagedCollection<EventUser>> Handle(EventUserGetPagedQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageNumber));

            if (request.PageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageSize));

            return await _unitOfWork.EventUserRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken).ConfigureAwait(false);
        }
    }
}
