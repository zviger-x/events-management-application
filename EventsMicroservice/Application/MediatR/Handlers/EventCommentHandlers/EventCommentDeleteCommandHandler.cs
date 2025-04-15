using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using MediatR;
using Shared.Caching.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentDeleteCommandHandler : BaseHandler, IRequestHandler<EventCommentDeleteCommand>
    {
        public EventCommentDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task Handle(EventCommentDeleteCommand request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.EventCommentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (comment == null)
                return;

            var isNotAuthor = request.CurrentUserId != comment.UserId;
            var isWrongEvent = request.RouteEventId != comment.EventId;

            if (isNotAuthor || isWrongEvent)
                throw new ParameterException("You are not allowed to delete this comment for the event.");

            await _unitOfWork.EventCommentRepository.DeleteAsync(comment, cancellationToken);
        }
    }
}
