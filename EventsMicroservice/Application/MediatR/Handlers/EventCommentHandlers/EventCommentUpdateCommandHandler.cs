using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using ArgumentException = Shared.Exceptions.ServerExceptions.ArgumentException;
using ArgumentNullException = Shared.Exceptions.ServerExceptions.ArgumentNullException;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentUpdateCommandHandler : BaseHandler<EventComment>, IRequestHandler<EventCommentUpdateCommand>
    {
        public EventCommentUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IEventCommentValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Handle(EventCommentUpdateCommand request, CancellationToken cancellationToken)
        {
            if (request.EventComment == null)
                throw new ArgumentNullException(nameof(request.EventComment));

            // TODO: Добавить проверку, что пользователь является создателем отзыва.

            if (request.RouteEventId != request.EventComment.EventId)
                throw new ArgumentException("You are not allowed to edit the comment for this event.");

            if (request.RouteCommentId != request.EventComment.Id)
                throw new ArgumentException("You are not allowed to modify this comment.");

            await _validator.ValidateAndThrowAsync(request.EventComment, cancellationToken);

            await _unitOfWork.EventCommentRepository.UpdateAsync(request.EventComment, cancellationToken).ConfigureAwait(false);
        }
    }
}
