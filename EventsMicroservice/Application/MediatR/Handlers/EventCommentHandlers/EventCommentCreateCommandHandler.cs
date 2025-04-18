using Application.Contracts;
using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentCreateCommandHandler : BaseHandler<CreateEventCommentDto>, IRequestHandler<EventCommentCreateCommand, Guid>
    {
        public EventCommentCreateCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            ICreateEventCommentDtoValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task<Guid> Handle(EventCommentCreateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.EventComment, cancellationToken);

            if (request.RouteEventId != request.EventComment.EventId)
                throw new ParameterException("You are not allowed to create a comment for this event.");

            // TODO: Добавить проверку на наличие пользователя (gRPC)

            var eventComment = _mapper.Map<EventComment>(request.EventComment);

            return await _unitOfWork.EventCommentRepository.CreateAsync(eventComment, cancellationToken);
        }
    }
}
