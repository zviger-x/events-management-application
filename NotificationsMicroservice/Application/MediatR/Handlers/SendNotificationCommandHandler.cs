using Application.Clients;
using Application.MediatR.Commands;
using Application.SignalR;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Caching.Services.Interfaces;

#error СДЕЛАТЬ НОРМАЛЬНЫЙ КЛЮЧ
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

            _logger.LogInformation("Handling SendNotificationCommand for user {UserId}", notification.UserId);

            var isSaved = await _userClient.TrySaveNotificationAsync(notification, cancellationToken);

            if (isSaved)
            {
                await _notificationSender.SendAsync(notification, cancellationToken);

                _logger.LogInformation("Notification sent to user {UserId}", notification.UserId);

                return;
            }

            var cacheKey = $"notification:failed:{notification.InCacheId}";

            await _cacheService.SetAsync(cacheKey, notification, true, cancellationToken);
            await _cacheService.AddToSetAsync("FailedSetKey", cacheKey, cancellationToken);

            _logger.LogWarning("Failed to save notification. Cached for retry. UserId: {UserId}", notification.UserId);
        }
    }
}
