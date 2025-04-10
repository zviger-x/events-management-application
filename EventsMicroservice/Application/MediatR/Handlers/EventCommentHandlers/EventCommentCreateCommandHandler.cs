using Application.MediatR.Commands.EventCommentCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentCreateCommandHandler : BaseHandler<EventComment>, IRequestHandler<EventCommentCreateCommand, Guid>
    {
        public EventCommentCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IEventCommentValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<Guid> Handle(EventCommentCreateCommand request, CancellationToken cancellationToken)
        {
            if (request.EventComment == null)
                throw new ParameterNullException(nameof(request.EventComment));

            if (request.RouteEventId != request.EventComment.EventId)
                throw new ParameterException("You are not allowed to create a comment for this event.");

            await _validator.ValidateAndThrowAsync(request.EventComment, cancellationToken);

            return await _unitOfWork.EventCommentRepository.CreateAsync(request.EventComment, cancellationToken).ConfigureAwait(false);
        }
    }
}
