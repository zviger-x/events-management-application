using Application.MediatR.Queries.EventCommentQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentGetByIdQueryHandler : BaseHandler, IRequestHandler<EventCommentGetByIdQuery, EventComment>
    {
        public EventCommentGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<EventComment> Handle(EventCommentGetByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.EventCommentRepository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
        }
    }
}
