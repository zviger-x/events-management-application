using Application.MediatR.Commands.EventUserCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserCreateCommandHandler : BaseHandler<EventUser>, IRequestHandler<EventUserCreateCommand, Guid>
    {
        public EventUserCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IEventUserValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task<Guid> Handle(EventUserCreateCommand request, CancellationToken cancellationToken)
        {
            var eventUser = new EventUser
            {
                EventId = request.EventId,
                UserId = request.UserId,
                SeatId = request.SeatId,
                RegisteredAt = DateTime.UtcNow
            };

            await _validator.ValidateAndThrowAsync(eventUser, cancellationToken);

            // Проверка на существование ивента с таким ID
            var @event = await _unitOfWork.EventRepository.GetByIdAsync(eventUser.EventId, cancellationToken);
            if (@event == null)
                throw new ParameterException("There is no event with this Id.");

            // Проверка на существование места на ивенте с таким ID
            var seat = await _unitOfWork.SeatRepository.GetByIdAsync(eventUser.SeatId, cancellationToken);
            if (seat == null)
                throw new ParameterException("There is no seat with this Id.");

            // Проверка принадлежит ли место к этому ивенту
            if (seat.EventId != @event.Id)
                throw new ParameterException("This seat does not belong to this event");

            // Проверка на то, что место куплено
            if (seat.IsBought)
                throw new ParameterException("This seat has already been bought");

            // TODO: Добавить проверку наличия пользователя (gRPC запрос)

            // Проверка подписан ли пользователь на этот ивент.
            // TODO: А нужна ли проверка на то, что пользователь подписан на ивент? Может в будущем реализовать возможность покупки нескольких билетов?
            var isRegistered = (await _unitOfWork.EventUserRepository.GetAllAsync(cancellationToken)).Any(e => e.UserId == request.UserId);
            if (isRegistered)
                throw new ParameterException("User is already registered.");

            // Помечаем сиденье купленным и создаём "ивент" пользователя
            // Возвращаем из транзакции ID созданного EventUser
            return await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                seat.IsBought = true;
                await _unitOfWork.SeatRepository.UpdateAsync(seat);

                return await _unitOfWork.EventUserRepository.CreateAsync(eventUser, cancellationToken);
            }, cancellationToken);
        }
    }
}
