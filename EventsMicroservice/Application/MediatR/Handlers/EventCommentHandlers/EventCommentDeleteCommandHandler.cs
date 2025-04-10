using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using MediatR;

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
            var comment = await _unitOfWork.EventCommentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (comment == null)
                return;

            await _unitOfWork.EventCommentRepository.DeleteAsync(comment, cancellationToken).ConfigureAwait(false);
        }
    }
}
