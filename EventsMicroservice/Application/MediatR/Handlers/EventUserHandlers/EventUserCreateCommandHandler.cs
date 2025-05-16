using Application.Caching.Constants;
using Application.MediatR.Commands.EventUserCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserCreateCommandHandler : BaseHandler, IRequestHandler<EventUserCreateCommand, Guid>
    {
        private readonly TimeSpan _lockTtl = TimeSpan.FromMinutes(5);
        private readonly IRedisCacheService _redisCacheService;

        public EventUserCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisCacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
            _redisCacheService = cacheService;
        }

        public async Task<Guid> Handle(EventUserCreateCommand request, CancellationToken cancellationToken)
        {
            var lockKey = CacheKeys.SeatLockKey(request.EventUser.SeatId);  // > SeatId
            var lockValue = request.EventUser.SeatId.ToString();            // > SeatId

            // Пытаюсь получить блокирующий объект
            await using var acquiredLock = await _redisCacheService.AcquireLockAsync(lockKey, lockValue, _lockTtl, cancellationToken);

            // Если получить не удалось, значит кто-то уже занял этот объект, выбрасываем ошибку
            if (acquiredLock == null)
                throw new ConflictException("This seat is currently being reserved by another user. Please try again later.");

            // Выполняю основной запрос
            return await HandleCreation(request, cancellationToken);
        }

        private async Task<Guid> HandleCreation(EventUserCreateCommand request, CancellationToken cancellationToken)
        {
            var eventUser = _mapper.Map<EventUser>(request.EventUser);

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
            // TODO: Что-то сделать здесь или забить. Ведь нужна ли в действительности проверка на то, что пользователь подписан на ивент?
            var isRegistered = (await _unitOfWork.EventUserRepository.GetAllAsync(cancellationToken)).Any(e => e.UserId == eventUser.UserId);
            if (isRegistered)
                throw new ParameterException("User is already registered.");

            // TODO: gRPC запрос в Payment Microservice для обработки покупки по токену
            var paymentResult = true;
            if (!paymentResult)
                throw new PaymentException("Failed to complete payment. Please try again or use another card.");

            try
            {
                // Помечаем сиденье купленным и создаём "ивент" пользователя
                // Возвращаем из транзакции ID созданного EventUser
                return await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
                {
                    seat.IsBought = true;
                    await _unitOfWork.SeatRepository.UpdateAsync(seat, token);

                    return await _unitOfWork.EventUserRepository.CreateAsync(eventUser, token);
                }, cancellationToken);
            }
            catch
            {
                // TODO: Сделать что-то в случаи успешной оплаты и не успешной покупки. Например "вернуть" деньги.
                throw;
            }
        }
    }
}
