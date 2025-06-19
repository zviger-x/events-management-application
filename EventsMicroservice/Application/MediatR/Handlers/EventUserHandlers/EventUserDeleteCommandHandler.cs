using Application.MediatR.Commands.EventUserCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserDeleteCommandHandler : BaseHandler, IRequestHandler<EventUserDeleteCommand>
    {
        public EventUserDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task Handle(EventUserDeleteCommand request, CancellationToken cancellationToken)
        {
            var eventUser = await _unitOfWork.EventUserRepository.GetByUserAndEventAsync(request.UserId, request.EventId, cancellationToken);
            if (eventUser == null)
                throw new NotFoundException("Event user is already deleted or not found");

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
