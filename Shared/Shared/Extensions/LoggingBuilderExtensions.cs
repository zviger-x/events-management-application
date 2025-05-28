using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Shared.Logging;

namespace Shared.Extensions
{
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// Configures Serilog logging for the application, allowing output to console, file, and Logstash.
        /// </summary>
        /// <param name="logging">The logging builder to configure.</param>
        /// <param name="microserviceName">The name of the microservice, used to enrich log events with a "Microservice" property.</param>
        /// <param name="consoleMinimumLevel">Minimum log level for console output (default: Verbose).</param>
        /// <param name="writeToFile">Indicates whether to write logs to a file (default: true).</param>
        /// <param name="logFilePath">Path to the log file (default: "logs/log.txt").</param>
        /// <param name="fileMinimumLevel">Minimum log level for file output (default: Verbose).</param>
        /// <param name="writeToLogstash">Indicates whether to send logs to Logstash (default: false).</param>
        /// <param name="logstashUri">The URI of the Logstash endpoint (e.g., "http://localhost:8098").</param>
        /// <param name="logstashMinimumLevel">Minimum log level for Logstash output (default: Verbose).</param>
        /// <returns>The configured <see cref="ILoggingBuilder"/> instance.</returns>
        public static ILoggingBuilder ConfigureLogger(
            this ILoggingBuilder logging,
            string microserviceName,
            LogEventLevel consoleMinimumLevel = LogEventLevel.Verbose,
            bool writeToFile = true,
            string logFilePath = "logs/log.txt",
            LogEventLevel fileMinimumLevel = LogEventLevel.Verbose,
            bool writeToLogstash = false,
            string logstashUri = null,
            LogEventLevel logstashMinimumLevel = LogEventLevel.Verbose)
        {
            var configuration = new LoggerConfiguration();

            // ServiceName
            configuration.Enrich.WithProperty("Microservice", microserviceName);

            // Console
            configuration.WriteTo.Console(theme: CustomConsoleThemes.SixteenEnhanced, restrictedToMinimumLevel: consoleMinimumLevel);

            // File
            if (writeToFile && !string.IsNullOrEmpty(logFilePath))
                configuration.WriteTo.File(
                    path: logFilePath,
                    restrictedToMinimumLevel: fileMinimumLevel,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7);

            // Logstash
            if (writeToLogstash && !string.IsNullOrEmpty(logstashUri))
                configuration.WriteTo.Http(
                    requestUri: logstashUri,
                    queueLimitBytes: null,
                    restrictedToMinimumLevel: logstashMinimumLevel);

            Log.Logger = configuration.CreateLogger();

            logging.ClearProviders();
            logging.AddSerilog();

            return logging;
        }
    }
}
