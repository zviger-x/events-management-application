using Application.Contracts;
using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentUpdateCommandHandler : BaseHandler<UpdateEventCommentDto>, IRequestHandler<EventCommentUpdateCommand>
    {
        public EventCommentUpdateCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            IValidator<UpdateEventCommentDto> validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task Handle(EventCommentUpdateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.EventComment, cancellationToken);

            var storedEventComment = await _unitOfWork.EventCommentRepository.GetByIdAsync(request.CommentId, cancellationToken);
            if (storedEventComment == null)
                throw new NotFoundException("Comment not found.");

            var currentUserId = request.CurrentUserId;
            var isAdmin = request.IsCurrentUserAdmin;
            var isAuthor = currentUserId == storedEventComment.UserId;
            var isWrongEvent = request.EventId != storedEventComment.EventId;

            var isCommentMismatchOrForbidden = isWrongEvent || (!isAuthor && !isAdmin);

            if (isCommentMismatchOrForbidden)
                throw new ForbiddenAccessException("You are not allowed to edit this comment for the event.");

            var eventComment = _mapper.Map(request.EventComment, storedEventComment);

            await _unitOfWork.EventCommentRepository.UpdateAsync(eventComment, cancellationToken);
        }
    }
}
