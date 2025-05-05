using Application.Caching.Constants;
using Application.Contracts;
using Application.MediatR.Commands;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Caching.Services.Interfaces;

namespace Application.Services.Background
{
    public class NotificationRetrySaveService : BackgroundService
    {
        private const int MaxDegreeOfParallelism = 5;

        private readonly IRedisCacheService _cacheService;
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationRetrySaveService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public NotificationRetrySaveService(
            IRedisCacheService cacheService,
            IMediator mediator,
            ILogger<NotificationRetrySaveService> logger)
        {
            _cacheService = cacheService;
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Notification retry background service started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Для начала получаем ожидающие уведомления
                    var cacheSetKey = CacheKeys.FailedNotificationsSet;

                    var failedNotifications = await _cacheService.GetSetMembersAsync<NotificationDto>(cacheSetKey, cancellationToken);

                    // Если их нет, пропускаем текущую итерацию
                    if (!failedNotifications.Any())
                    {
                        await Task.Delay(_interval, cancellationToken);
                        continue;
                    }

                    // Если они есть, то в через Parallel.ForEachAsync пытаемся их переотправить.
                    _logger.LogInformation("Found {Count} failed notifications to retry", failedNotifications.Count());

                    var parallelOptions = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = MaxDegreeOfParallelism,
                        CancellationToken = cancellationToken
                    };

                    await Parallel.ForEachAsync(failedNotifications, parallelOptions,
                        async (notification, token) =>
                        {
                            try
                            {
                                var command = new SendNotificationCommand { Notification = notification };

                                await _mediator.Send(command, cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to resend notification for user {UserId}", notification.UserId);
                            }
                        });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while retrying notifications");
                }

                await Task.Delay(_interval, cancellationToken);
            }

            _logger.LogInformation("Notification retry background service stopped.");
        }
    }
}
