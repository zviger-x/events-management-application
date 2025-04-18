using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using MediatR;
using Shared.Caching.Services.Interfaces;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventDeleteCommandHandler : BaseHandler, IRequestHandler<EventDeleteCommand>
    {
        public EventDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task Handle(EventDeleteCommand request, CancellationToken cancellationToken)
        {
            var @event = await _unitOfWork.EventRepository.GetByIdAsync(request.Id, cancellationToken);
            if (@event == null)
                return;

            await _unitOfWork.EventRepository.DeleteAsync(@event, cancellationToken);
        }
    }
}
