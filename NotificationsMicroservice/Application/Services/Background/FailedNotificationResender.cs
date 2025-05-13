using Application.Caching.Constants;
using Application.Contracts;
using Application.MediatR.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Caching.Services.Interfaces;
using Shared.Services.Background;

namespace Application.Services.Background
{
    public class FailedNotificationResender : AdvancedBackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FailedNotificationResender> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public FailedNotificationResender(IServiceScopeFactory scopeFactory, ILogger<FailedNotificationResender> logger)
            : base(logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override Task InitializeAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Notification retry background service started.");

            return Task.CompletedTask;
        }

        protected async override Task ExecuteIterationAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(_interval, cancellationToken);

            try
            {
                using var scope = _scopeFactory.CreateScope();

                var cacheService = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // Для начала получаем ожидающие уведомления
                var cacheSetKey = CacheKeys.FailedNotificationsSet;

                var failedNotifications = await cacheService.GetSetMembersAsync<NotificationDto>(cacheSetKey, cancellationToken);

                // Если их нет, пропускаем текущую итерацию
                if (!failedNotifications.Any())
                    return;

                // Если они есть, то в через Parallel.ForEachAsync пытаемся их переотправить.
                _logger.LogInformation("Found {Count} failed notifications to retry", failedNotifications.Count());

                foreach (var notification in failedNotifications)
                {
                    try
                    {
#warning TODO: Убрать выброс ошибки!
                        var command = new SendNotificationCommand { Notification = notification, ThrowError = false };

                        await mediator.Send(command, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to resend notification for user {UserId}", notification.UserId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrying notifications");
            }
        }

        protected override Task CleanupAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Notification retry background service stopped.");

            return Task.CompletedTask;
        }
    }
}
