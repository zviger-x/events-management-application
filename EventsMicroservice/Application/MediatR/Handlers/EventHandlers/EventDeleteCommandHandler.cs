using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventDeleteCommandHandler : BaseHandler, IRequestHandler<EventDeleteCommand>
    {
        public EventDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task Handle(EventDeleteCommand request, CancellationToken cancellationToken)
        {
            var @event = new Event() { Id = request.Id };

            await _unitOfWork.EventRepository.DeleteAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
