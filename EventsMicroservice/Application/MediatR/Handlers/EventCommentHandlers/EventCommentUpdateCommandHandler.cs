using Application.Contracts;
using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;
using Shared.Services.Interfaces;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentUpdateCommandHandler : BaseHandler<UpdateEventCommentDto>, IRequestHandler<EventCommentUpdateCommand>
    {
        private readonly ICurrentUserService _currentUserService;

        public EventCommentUpdateCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            IUpdateEventCommentDtoValidator validator,
            ICurrentUserService currentUserService)
            : base(unitOfWork, mapper, cacheService, validator)
        {
            _currentUserService = currentUserService;
        }

        public async Task Handle(EventCommentUpdateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.EventComment, cancellationToken);

            var isWrongRouteId = request.RouteCommentId != request.EventComment.Id;
            if (isWrongRouteId)
                throw new ParameterException("You are not allowed to modify this comment.");

            var storedEventComment = await _unitOfWork.EventCommentRepository.GetByIdAsync(request.EventComment.Id, cancellationToken);
            if (storedEventComment == null)
                throw new NotFoundException("Comment not found.");

            var currentUserId = _currentUserService.GetUserIdOrThrow();
            var isAdmin = _currentUserService.IsAdminOrThrow();
            var isAuthor = currentUserId == storedEventComment.UserId;
            var isWrongEvent = request.RouteEventId != storedEventComment.EventId;

            if (isWrongEvent || (!isAuthor && !isAdmin))
                throw new ForbiddenAccessException("You are not allowed to edit this comment for the event.");

            var eventComment = _mapper.Map(request.EventComment, storedEventComment);

            await _unitOfWork.EventCommentRepository.UpdateAsync(eventComment, cancellationToken);
        }
    }
}
