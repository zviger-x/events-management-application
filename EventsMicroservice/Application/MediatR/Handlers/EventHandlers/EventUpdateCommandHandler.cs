using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventUpdateCommandHandler : BaseHandler<UpdateEventDto>, IRequestHandler<EventUpdateCommand>
    {
        public EventUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUpdateEventDtoValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Handle(EventUpdateCommand request, CancellationToken cancellationToken)
        {
            if (request.Event == null)
                throw new ParameterNullException(nameof(request.Event));

            if (request.RouteEventId != request.Event.Id)
                throw new ParameterException("You are not allowed to modify this event.");

            await _validator.ValidateAndThrowAsync(request.Event, cancellationToken);

            var @event = _mapper.Map<Event>(request.Event);
            await _unitOfWork.EventRepository.UpdateAsync(@event, cancellationToken);
        }
    }
}
