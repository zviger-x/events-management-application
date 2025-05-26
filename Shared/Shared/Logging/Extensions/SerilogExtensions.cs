using Logger = Serilog.Log;

namespace Shared.Logging.Extensions
{
    public static class SerilogExtensions
    {
        public static class Log
        {
            // ===== DEBUG =====
            public static void DebugInterpolated(FormattableString message) =>
                Logger.Debug(message.Format, message.GetArguments());

            public static void DebugInterpolated(Exception exception, FormattableString message) =>
                Logger.Debug(exception, message.Format, message.GetArguments());

            public static void Debug(string message, params object[] args) =>
                Logger.Debug(message, args);

            public static void Debug(Exception exception, string message, params object[] args) =>
                Logger.Debug(exception, message, args);

            // ===== INFORMATION =====
            public static void InformationInterpolated(FormattableString message) =>
                Logger.Information(message.Format, message.GetArguments());

            public static void InformationInterpolated(Exception exception, FormattableString message) =>
                Logger.Information(exception, message.Format, message.GetArguments());

            public static void Information(string message, params object[] args) =>
                Logger.Information(message, args);

            public static void Information(Exception exception, string message, params object[] args) =>
                Logger.Information(exception, message, args);

            // ===== WARNING =====
            public static void WarningInterpolated(FormattableString message) =>
                Logger.Warning(message.Format, message.GetArguments());

            public static void WarningInterpolated(Exception exception, FormattableString message) =>
                Logger.Warning(exception, message.Format, message.GetArguments());

            public static void Warning(string message, params object[] args) =>
                Logger.Warning(message, args);

            public static void Warning(Exception exception, string message, params object[] args) =>
                Logger.Warning(exception, message, args);

            // ===== ERROR =====
            public static void ErrorInterpolated(FormattableString message) =>
                Logger.Error(message.Format, message.GetArguments());

            public static void ErrorInterpolated(Exception exception, FormattableString message) =>
                Logger.Error(exception, message.Format, message.GetArguments());

            public static void Error(string message, params object[] args) =>
                Logger.Error(message, args);

            public static void Error(Exception exception, string message, params object[] args) =>
                Logger.Error(exception, message, args);

            // ===== FATAL =====
            public static void FatalInterpolated(FormattableString message) =>
                Logger.Fatal(message.Format, message.GetArguments());

            public static void FatalInterpolated(Exception exception, FormattableString message) =>
                Logger.Fatal(exception, message.Format, message.GetArguments());

            public static void Fatal(string message, params object[] args) =>
                Logger.Fatal(message, args);

            public static void Fatal(Exception exception, string message, params object[] args) =>
                Logger.Fatal(exception, message, args);
        }
    }
}
