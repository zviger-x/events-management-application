using Application.Caching.Constants;
using Application.Clients;
using Application.MediatR.Commands;
using Application.SignalR;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Caching.Services.Interfaces;

namespace Application.MediatR.Handlers
{
    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand>
    {
        private readonly IUserNotificationsClient _userClient;
        private readonly IRedisCacheService _cacheService;
        private readonly INotificationSender _notificationSender;
        private readonly ILogger<SendNotificationCommandHandler> _logger;

        public SendNotificationCommandHandler(
            IUserNotificationsClient userClient,
            IRedisCacheService cacheService,
            INotificationSender notificationSender,
            ILogger<SendNotificationCommandHandler> logger)
        {
            _userClient = userClient;
            _cacheService = cacheService;
            _notificationSender = notificationSender;
            _logger = logger;
        }

        public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = request.Notification;

            _logger.LogInformation("Handling send notification command for user {UserId}", notification.UserId);

            // Пытаемся сохранить уведомление в базе данных на стороне микросервиса пользователей
            var isSaved = await _userClient.TrySaveNotificationAsync(notification, cancellationToken);

#warning TODO: Убрать выброс ошибки!
            if (request.ThrowError)
                isSaved = false;

            // Если сохранилось, отправляем по SignalR уведомление
            // и удаляем из кэша записи о неудачной попытке (если есть)
            if (isSaved)
            {
                await _notificationSender.SendAsync(notification, cancellationToken);

                _logger.LogInformation("Notification sent to user {UserId}", notification.UserId);

                var key = CacheKeys.FailedNotification(notification.InCacheId);
                await _cacheService.RemoveAsync(key, cancellationToken);
                await _cacheService.RemoveFromSetAsync(CacheKeys.FailedNotificationsSet, key, cancellationToken);

                return;
            }

            // Если сохранить не удалось, мы должны его переотправить
            // Для этого закэшируем его по его идентификатору InCacheId, а так же добавм этот же айди во множество
            // Остальная работа ляжет на бэкграунд сервис для повторной отправки,
            // который достанет из кэша и вызовет этот же хэндлер для полученного уведомления.

            var cacheKey = CacheKeys.FailedNotification(notification.InCacheId);
            var cacheSetKey = CacheKeys.FailedNotificationsSet;

            await _cacheService.SetAsync(cacheKey, notification, true, cancellationToken);
            await _cacheService.AddToSetAsync(cacheSetKey, cacheKey, cancellationToken);

            _logger.LogWarning("Failed to save notification. Cached for retry. UserId: {UserId}", notification.UserId);
        }
    }
}
