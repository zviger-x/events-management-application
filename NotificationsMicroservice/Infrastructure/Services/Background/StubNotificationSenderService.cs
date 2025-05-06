using Application.Contracts;
using Application.MediatR.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Background
{
    public class StubNotificationSenderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StubNotificationSenderService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(15);

        public StubNotificationSenderService(IServiceScopeFactory scopeFactory, ILogger<StubNotificationSenderService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(_interval / 2, cancellationToken);

            _logger.LogInformation("Stub Notification Sender Service started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, cancellationToken);

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var command = new SendNotificationCommand
                    {
                        Notification = new NotificationDto
                        {
                            UserId = Guid.Parse("4bef6735-b9eb-446f-2382-08dd75c5f04b"),
                            Message = $"[Stub] Test notification at {DateTime.UtcNow:T}"
                        }
                    };

                    _logger.LogInformation("Sending stub notification to user {UserId}", command.Notification.UserId);

                    await mediatr.Send(command, cancellationToken);
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
