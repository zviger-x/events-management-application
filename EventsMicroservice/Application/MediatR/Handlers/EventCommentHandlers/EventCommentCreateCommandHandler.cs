using Application.Clients;
using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentCreateCommandHandler : BaseHandler, IRequestHandler<EventCommentCreateCommand, Guid>
    {
        private readonly IUserClient _userClient;

        public EventCommentCreateCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            IUserClient userClient)
            : base(unitOfWork, mapper, cacheService)
        {
            _userClient = userClient;
        }

        public async Task<Guid> Handle(EventCommentCreateCommand request, CancellationToken cancellationToken)
        {
            var isUserExists = await _userClient.UserExistsAsync(request.EventComment.UserId, cancellationToken);
            if (!isUserExists)
                throw new NotFoundException("The specified user does not exist.");

            var eventComment = _mapper.Map<EventComment>(request.EventComment);
            eventComment.EventId = request.EventId;

            return await _unitOfWork.EventCommentRepository.CreateAsync(eventComment, cancellationToken);
        }
    }
}
