using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;
using Shared.Services.Interfaces;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentDeleteCommandHandler : BaseHandler, IRequestHandler<EventCommentDeleteCommand>
    {
        private readonly ICurrentUserService _currentUserService;

        public EventCommentDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, ICurrentUserService currentUserService)
            : base(unitOfWork, mapper, cacheService)
        {
            _currentUserService = currentUserService;
        }

        public async Task Handle(EventCommentDeleteCommand request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.EventCommentRepository.GetByIdAsync(request.CommentId, cancellationToken);
            if (comment == null)
                return;

            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isAuthor = currentUserId == comment.UserId;
            var isWrongEvent = request.EventId != comment.EventId;

            var isCommentMismatchOrUnauthorized = isWrongEvent || (!isAuthor && !isAdmin);

            if (isCommentMismatchOrUnauthorized)
                throw new ForbiddenAccessException("You are not allowed to delete this comment for the event.");

            await _unitOfWork.EventCommentRepository.DeleteAsync(comment, cancellationToken);
        }
    }
}
