using Application.Contracts;
using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Services.Interfaces;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentCreateCommandHandler : BaseHandler<CreateEventCommentDto>, IRequestHandler<EventCommentCreateCommand, Guid>
    {
        public EventCommentCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IValidator<CreateEventCommentDto> validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task<Guid> Handle(EventCommentCreateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.EventComment, cancellationToken);

            // TODO: Добавить проверку на наличие пользователя (gRPC)

            var eventComment = _mapper.Map<EventComment>(request.EventComment);
            eventComment.EventId = request.EventId;

            return await _unitOfWork.EventCommentRepository.CreateAsync(eventComment, cancellationToken);
        }
    }
}
