using Application.Caching.Constants;
using Application.Contracts;
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

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserCreateCommandHandler : BaseHandler<CreateEventUserDto>, IRequestHandler<EventUserCreateCommand, Guid>
    {
        private const int LockTtlInMinutes = 5;
        private const int LockTtlUpdateDelayInMinutes = 4;

        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger<EventUserCreateCommandHandler> _logger;

        public EventUserCreateCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            IRedisCacheService redisCacheService,
            ICreateEventUserDtoValidator validator,
            ILogger<EventUserCreateCommandHandler> logger)
            : base(unitOfWork, mapper, redisCacheService, validator)
        {
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        public async Task<Guid> Handle(EventUserCreateCommand request, CancellationToken cancellationToken)
        {
            // Создаю объект-блокировщий, чтобы не было одновременного доступа к месту
            var lockKey = CacheKeys.SeatLockKey(request.EventUser.SeatId);
            var lockValue = request.EventUser.SeatId.ToString();
            var lockTtl = TimeSpan.FromMinutes(LockTtlInMinutes);
            var lockTtlUpdateDelay = TimeSpan.FromMinutes(LockTtlUpdateDelayInMinutes);

            var acquiredLock = await _redisCacheService.AcquireLockAsync(lockKey, lockValue, lockTtl, cancellationToken);
            if (!acquiredLock)
                throw new ConflictException("This seat is currently being reserved by another user. Please try again later.");

            // Создаю отдельный связанный токен с оригинальным и запускаю задачу по обновлению объекта-блокировки
            // Это нужно, чтобы доступ не был выдан раньше времени, если возникнет непредвиденная задержка и объект истечёт
            using var extendCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var ttlExpender = RunTtlExpander(lockKey, lockValue, lockTtl, lockTtlUpdateDelay, extendCts.Token);

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
                await _redisCacheService.ReleaseLockAsync(lockKey, cancellationToken);
            }
        }

        private Task RunTtlExpander(string lockKey, string lockValue, TimeSpan lockTtl, TimeSpan delayBetweenUpdates, CancellationToken cancellationToken)
        {
            if (delayBetweenUpdates >= lockTtl)
            {
                _logger.LogWarning(
                    "The delay between TTL updates ({0} seconds) is equal to or greater than the lock TTL ({1} seconds). " +
                    "This may result in the expired lock being re-created. Therefore, the TTL update task was not started.",
                    delayBetweenUpdates.TotalSeconds,
                    lockTtl.TotalSeconds);

                return Task.CompletedTask;
            }

            return Task.Run(async () =>
            {
                _logger.LogInformation($"The task to extend the life of TTL for key {lockKey} has been launched.");

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // Интервал между обновлениями
                        await Task.Delay(delayBetweenUpdates, cancellationToken);

                        // Обновляем TTL блокировки, продлевая его время жизни
                        await _redisCacheService.RefreshKeyTtlAsync(lockKey, lockTtl, cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogInformation($"The task to extend the life of TTL for key {lockKey} has been completed.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Failed to update TTL for key {lockKey}: {ex.Message}");
                    }
                }
            }, cancellationToken);
        }

        private async Task<Guid> HandleCreation(EventUserCreateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.EventUser, cancellationToken);

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
