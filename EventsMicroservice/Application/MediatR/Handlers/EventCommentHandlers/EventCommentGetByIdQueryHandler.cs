using Application.MediatR.Queries.EventCommentQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentGetByIdQueryHandler : BaseHandler, IRequestHandler<EventCommentGetByIdQuery, EventComment>
    {
        public EventCommentGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<EventComment> Handle(EventCommentGetByIdQuery request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.EventCommentRepository.GetByIdAsync(request.Id, cancellationToken);
            return comment ?? throw new NotFoundException("Comment not found.");
        }
    }
}
