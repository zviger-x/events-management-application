using Application.Contracts;
using Application.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Background
{
    public class StubNotificationSenderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StubNotificationSenderService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(20);

        public StubNotificationSenderService(IServiceScopeFactory scopeFactory, ILogger<StubNotificationSenderService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stub Notification Sender Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, stoppingToken);

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var sender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

                    var testNotification = new NotificationDto
                    {
                        UserId = Guid.NewGuid(),
                        Message = $"[Stub] Test notification at {DateTime.UtcNow:T}"
                    };

                    await sender.SendAsync(testNotification, stoppingToken);

                    _logger.LogInformation("Sent stub notification to user {UserId}", testNotification.UserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send stub notification");
                }
            }

            _logger.LogInformation("Stub Notification Sender Service stopped.");
        }
    }
}
