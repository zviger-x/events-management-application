using Application.Contracts;
using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using Shared.Caching.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentUpdateCommandHandler : BaseHandler<UpdateEventCommentDto>, IRequestHandler<EventCommentUpdateCommand>
    {
        public EventCommentUpdateCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            IUpdateEventCommentDtoValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task Handle(EventCommentUpdateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.EventComment, cancellationToken);
         
            // TODO: Добавить проверку, что пользователь является создателем отзыва.

            if (request.RouteEventId != request.EventComment.EventId)
                throw new ParameterException("You are not allowed to edit the comment for this event.");

            if (request.RouteCommentId != request.EventComment.Id)
                throw new ParameterException("You are not allowed to modify this comment.");

            var storedEventComment = await _unitOfWork.EventCommentRepository.GetByIdAsync(request.EventComment.Id, cancellationToken);
            if (storedEventComment == null)
                throw new NotFoundException("Comment not found.");

            var eventComment = _mapper.Map(request.EventComment, storedEventComment);

            await _unitOfWork.EventCommentRepository.UpdateAsync(eventComment, cancellationToken);
        }
    }
}
