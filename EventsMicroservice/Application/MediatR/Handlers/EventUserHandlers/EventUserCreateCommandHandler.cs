using Application.Caching.Constants;
using Application.MediatR.Commands.EventUserCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Caching.Interfaces;
using Shared.Exceptions.ServerExceptions;
using System.Threading;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserCreateCommandHandler : BaseHandler<EventUser>, IRequestHandler<EventUserCreateCommand, Guid>
    {
        private const int LockTtlInMinutes = 5;
        private const int LockTtlUpdateDelayInMinutes = 4;

        private readonly ILogger<EventUserCreateCommandHandler> _logger;

        public EventUserCreateCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            IEventUserValidator validator,
            ILogger<EventUserCreateCommandHandler> logger)
            : base(unitOfWork, mapper, cacheService, validator)
        {
            _logger = logger;
        }

        public async Task<Guid> Handle(EventUserCreateCommand request, CancellationToken cancellationToken)
        {
            // Создаю объект-блокировщий, чтобы не было одновременного доступа к месту
            var lockKey = CacheKeys.SeatLockKey(request.SeatId);
            var lockValue = request.SeatId.ToString();
            var lockTtl = TimeSpan.FromMinutes(LockTtlInMinutes);
            var lockTtlUpdateDelay = TimeSpan.FromMinutes(LockTtlUpdateDelayInMinutes);

            var cachedLock = await _cacheService.GetAsync<string>(lockKey, cancellationToken);
            if (cachedLock != null)
                throw new ConflictException("This seat is currently being reserved by another user. Please try again later.");

            await _cacheService.SetAsync(lockKey, lockValue, lockTtl, cancellationToken);

            // Создаю отдельный связанный токен с оригинальным и запускаю задачу по обновлению объекта-блокировки
            // Это нужно, чтобы доступ не был выдан раньше времени, если возникнет непредвиденная задержка и объект истечёт
            using var extendCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var ttlExpender = RunTtlExpander(lockKey, lockValue, lockTtl, lockTtlUpdateDelay, extendCts);

            try
            {
                // Основной запрос
                return await HandleCreation(request, cancellationToken);
            }
            finally
            {
                // Останавливаю фоновую задачу и удаляю объект-блокировщик из кэша
                extendCts.Cancel();
                await ttlExpender;
                await _cacheService.RemoveAsync(lockKey, cancellationToken);
            }
        }

        private Task RunTtlExpander(string lockKey, string lockValue, TimeSpan lockTtl, TimeSpan delayBetweenUpdates, CancellationTokenSource extendCts)
        {
            // Фоновая задача, обновляющая TTL каждые 2 минуты, пока не завершится основная операция
            return Task.Run(async () =>
            {
                while (!extendCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        // Ждём некоторое время перед следующим продлением
                        await Task.Delay(delayBetweenUpdates, extendCts.Token);

                        // Обновляем TTL блокировки, продлевая его время жизни
                        await _cacheService.SetAsync(lockKey, lockValue, lockTtl, extendCts.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Failed to update TTL for key {lockKey}: {ex.Message}");
                    }
                }
            }, extendCts.Token);
        }

        private async Task<Guid> HandleCreation(EventUserCreateCommand request, CancellationToken cancellationToken)
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
                    await _unitOfWork.SeatRepository.UpdateAsync(seat);

                    return await _unitOfWork.EventUserRepository.CreateAsync(eventUser, cancellationToken);
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
