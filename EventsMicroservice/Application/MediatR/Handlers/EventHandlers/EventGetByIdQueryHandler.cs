using Application.MediatR.Queries.EventQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventGetByIdQueryHandler : BaseHandler, IRequestHandler<EventGetByIdQuery, Event>
    {
        public EventGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<Event> Handle(EventGetByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EventRepository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
        }
    }
}
