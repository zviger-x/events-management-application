using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using MediatR;
using Shared.Caching.Services.Interfaces;
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
            var comment = await _unitOfWork.EventCommentRepository.GetByIdAsync(request.CommentId, cancellationToken);
            if (comment == null)
                throw new NotFoundException("Comment is already deleted or not found");

            var currentUserId = request.CurrentUserId;
            var isAdmin = request.IsCurrentUserAdmin;
            var isAuthor = currentUserId == comment.UserId;
            var isWrongEvent = request.EventId != comment.EventId;

            var isCommentMismatchOrForbidden = isWrongEvent || (!isAuthor && !isAdmin);

            if (isCommentMismatchOrForbidden)
                throw new ForbiddenAccessException("You are not allowed to delete this comment for the event.");

            await _unitOfWork.EventCommentRepository.DeleteAsync(comment, cancellationToken);
        }
    }
}
