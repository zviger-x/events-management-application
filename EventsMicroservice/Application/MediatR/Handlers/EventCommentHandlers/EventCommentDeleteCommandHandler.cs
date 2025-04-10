using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using MediatR;
using ArgumentException = Shared.Exceptions.ServerExceptions.ArgumentException;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentDeleteCommandHandler : BaseHandler, IRequestHandler<EventCommentDeleteCommand>
    {
        public EventCommentDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task Handle(EventCommentDeleteCommand request, CancellationToken cancellationToken)
        {
            // TODO: Добавить проверку, что пользователь является создателем отзыва.

            var comment = await _unitOfWork.EventCommentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (comment == null)
                return;

            if (request.RouteEventId != comment.EventId)
                throw new ArgumentException("You are not allowed to delete this comment for the event.");

            await _unitOfWork.EventCommentRepository.DeleteAsync(comment, cancellationToken).ConfigureAwait(false);
        }
    }
}
