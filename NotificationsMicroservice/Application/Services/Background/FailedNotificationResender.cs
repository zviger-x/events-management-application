using Application.Caching.Constants;
using Application.Contracts;
using Application.MediatR.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Caching.Services.Interfaces;

namespace Application.Services.Background
{
    public class FailedNotificationResender : BackgroundService
    {
        private const int MaxDegreeOfParallelism = 5;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FailedNotificationResender> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public FailedNotificationResender(IServiceScopeFactory serviceScopeFactory, ILogger<FailedNotificationResender> logger)
        {
            _scopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Notification retry background service started.");

            while (!cancellationToken.IsCancellationRequested)
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

                    _logger.LogInformation("Found {Count} failed notifications to retry", failedNotifications.Count());

                    // Если их нет, пропускаем текущую итерацию
                    if (!failedNotifications.Any())
                        continue;

                    // Если они есть, то в через Parallel.ForEachAsync пытаемся их переотправить.
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

                                await mediator.Send(command, token);
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
            }

            _logger.LogInformation("Notification retry background service stopped.");
        }
    }
}
