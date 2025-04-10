using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventUpdateCommandHandler : BaseHandler<UpdateEventDTO>, IRequestHandler<EventUpdateCommand>
    {
        public EventUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUpdateEventDTOValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Handle(EventUpdateCommand request, CancellationToken cancellationToken)
        {
            if (request.Event == null)
                throw new ArgumentNullException(nameof(request.Event));

            await _validator.ValidateAndThrowAsync(request.Event, cancellationToken);

            var @event = _mapper.Map<Event>(request.Event);
            await _unitOfWork.EventRepository.UpdateAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
