using Application.MediatR.Commands.EventUserCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserDeleteCommandHandler : BaseHandler<EventUser>, IRequestHandler<EventUserDeleteCommand>
    {
        public EventUserDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IEventUserValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Handle(EventUserDeleteCommand request, CancellationToken cancellationToken)
        {
            var eventUser = new EventUser() { Id = request.Id };

            await _unitOfWork.EventUserRepository.DeleteAsync(eventUser, cancellationToken).ConfigureAwait(false);
        }
    }
}
