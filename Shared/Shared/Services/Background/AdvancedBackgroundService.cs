using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Services.Background
{
    public abstract class AdvancedBackgroundService : BackgroundService
    {
        protected bool RestartOnUnhandledException { get; set; }

        protected readonly ILogger<AdvancedBackgroundService> _logger;

        protected AdvancedBackgroundService(ILogger<AdvancedBackgroundService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Executed once before the loop starts. This is initialization.
        /// </summary>
        protected abstract Task InitializeAsync(CancellationToken cancellationToken);

        /// <summary>
        /// One iteration of the main loop. Called repeatedly through the <see langword="while"/> loop unless cancel is called or an error occurs.
        /// </summary>
        protected abstract Task ExecuteIterationAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Executed once after the cycle is completed. Here - cleaning.
        /// </summary>
        protected abstract Task CleanupAsync(CancellationToken cancellationToken);

        /// <summary>
        /// A single "skeleton" of the service's operation: initialization -> cycle -> cleanup.
        /// </summary>
        protected sealed override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var serviceName = GetType().Name;

            do
            {
                try
                {
                    _logger.LogInformation("Service initializing... Service: {service}", serviceName);
                    await InitializeAsync(cancellationToken);

                    _logger.LogInformation("Entering execution loop... Service: {service}", serviceName);
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await ExecuteIterationAsync(cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Service cancellation requested. Service: {service}", serviceName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception in service. Service: {service}", serviceName);

                    if (!RestartOnUnhandledException)
                        break;

                    _logger.LogInformation("Restarting service due to unhandled exception. Service: {service}", serviceName);

                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
                finally
                {
                    _logger.LogInformation("Service cleaning up... Service: {service}", serviceName);

                    await CleanupAsync(cancellationToken);
                }
            }
            while (RestartOnUnhandledException && !cancellationToken.IsCancellationRequested);

            _logger.LogInformation("Service stopped. Service: {service}", serviceName);
        }
    }
}
