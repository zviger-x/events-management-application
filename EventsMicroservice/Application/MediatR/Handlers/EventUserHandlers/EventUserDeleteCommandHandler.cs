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
            var eventUser = await _unitOfWork.EventUserRepository.GetByIdAsync(request.Id, cancellationToken);
            if (eventUser == null)
                return;

            var seat = await _unitOfWork.SeatRepository.GetByIdAsync(eventUser.SeatId, cancellationToken);
            if (seat == null)
                throw new ArgumentNullException(nameof(seat), "No seat found for which user is registered.");

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                seat.IsBought = false;
                await _unitOfWork.SeatRepository.UpdateAsync(seat, cancellationToken);

                await _unitOfWork.EventUserRepository.DeleteAsync(eventUser, cancellationToken).ConfigureAwait(false);
            });
        }
    }
}
