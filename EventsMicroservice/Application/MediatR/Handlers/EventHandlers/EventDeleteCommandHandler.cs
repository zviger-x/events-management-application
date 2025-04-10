using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
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
            var @event = await _unitOfWork.EventRepository.GetByIdAsync(request.Id, cancellationToken);
            if (@event == null)
                return;

            await _unitOfWork.EventRepository.DeleteAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
