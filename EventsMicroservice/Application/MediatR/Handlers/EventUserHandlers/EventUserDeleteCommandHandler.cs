using Application.MediatR.Commands.EventUserCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Services.Interfaces;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserDeleteCommandHandler : BaseHandler<EventUser>, IRequestHandler<EventUserDeleteCommand>
    {
        public EventUserDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IValidator<EventUser> validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task Handle(EventUserDeleteCommand request, CancellationToken cancellationToken)
        {
            var eventUser = await _unitOfWork.EventUserRepository.GetByUserAndEventAsync(request.UserId, request.EventId, cancellationToken);
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
