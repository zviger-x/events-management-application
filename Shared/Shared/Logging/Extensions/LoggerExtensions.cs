using Microsoft.Extensions.Logging;

namespace Shared.Logging.Extensions
{
    public static class LoggerExtensions
    {
        // ===== VERBOSE (Trace) =====
        public static void LogVerboseInterpolated(this ILogger logger, FormattableString message) =>
            logger.LogTrace(message.Format, message.GetArguments());

        public static void LogVerboseInterpolated(this ILogger logger, Exception exception, FormattableString message) =>
            logger.LogTrace(exception, message.Format, message.GetArguments());

        // ===== DEBUG =====
        public static void LogDebugInterpolated(this ILogger logger, FormattableString message) =>
            logger.LogDebug(message.Format, message.GetArguments());

        public static void LogDebugInterpolated(this ILogger logger, Exception exception, FormattableString message) =>
            logger.LogDebug(exception, message.Format, message.GetArguments());

        // ===== INFORMATION =====
        public static void LogInformationInterpolated(this ILogger logger, FormattableString message) =>
            logger.LogInformation(message.Format, message.GetArguments());

        public static void LogInformationInterpolated(this ILogger logger, Exception exception, FormattableString message) =>
            logger.LogInformation(exception, message.Format, message.GetArguments());

        // ===== WARNING =====
        public static void LogWarningInterpolated(this ILogger logger, FormattableString message) =>
            logger.LogWarning(message.Format, message.GetArguments());

        public static void LogWarningInterpolated(this ILogger logger, Exception exception, FormattableString message) =>
            logger.LogWarning(exception, message.Format, message.GetArguments());

        // ===== ERROR =====
        public static void LogErrorInterpolated(this ILogger logger, FormattableString message) =>
            logger.LogError(message.Format, message.GetArguments());

        public static void LogErrorInterpolated(this ILogger logger, Exception exception, FormattableString message) =>
            logger.LogError(exception, message.Format, message.GetArguments());

        // ===== CRITICAL =====
        public static void LogCriticalInterpolated(this ILogger logger, FormattableString message) =>
            logger.LogCritical(message.Format, message.GetArguments());

        public static void LogCriticalInterpolated(this ILogger logger, Exception exception, FormattableString message) =>
            logger.LogCritical(exception, message.Format, message.GetArguments());
    }
}
