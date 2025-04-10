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

            await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                var seat = await _unitOfWork.SeatRepository.GetByIdAsync(eventUser.SeatId, token);
                if (seat != null)
                {
                    seat.IsBought = false;
                    await _unitOfWork.SeatRepository.UpdateAsync(seat, token);
                }

                await _unitOfWork.EventUserRepository.DeleteAsync(eventUser, token);
            }, cancellationToken);
        }
    }
}
