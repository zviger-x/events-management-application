using Serilog.Sinks.SystemConsole.Themes;

namespace Shared.Logging
{
    public static class CustomConsoleThemes
    {
        public static AnsiConsoleTheme SixteenEnhanced { get; } = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "", // базовый текст — обычный
            [ConsoleThemeStyle.SecondaryText] = "\u001b[90m", // тёмно-серый
            [ConsoleThemeStyle.TertiaryText] = "\u001b[2m", // приглушенный серый

            [ConsoleThemeStyle.Invalid] = "\u001b[33;1m", // ярко-жёлтый
            [ConsoleThemeStyle.Null] = "\u001b[34m", // синий
            [ConsoleThemeStyle.Name] = "\u001b[36;1m", // бирюзовый
            [ConsoleThemeStyle.String] = "\u001b[36m", // бирюзовый
            [ConsoleThemeStyle.Number] = "\u001b[34m", // синий
            [ConsoleThemeStyle.Boolean] = "\u001b[34m", // синий
            [ConsoleThemeStyle.Scalar] = "\u001b[32m", // зелёный

            [ConsoleThemeStyle.LevelVerbose] = "\u001b[90m", // тёмно-серый
            [ConsoleThemeStyle.LevelDebug] = "\u001b[37m", // светло-серый
            [ConsoleThemeStyle.LevelInformation] = "\u001b[36;1m", // яркий бирюзовый
            [ConsoleThemeStyle.LevelWarning] = "\u001b[33;1m", // ярко-жёлтый
            [ConsoleThemeStyle.LevelError] = "\u001b[31;1m", // ярко-красный
            [ConsoleThemeStyle.LevelFatal] = "\u001b[41;1;97m" // белый текст на ярко-красном фоне
        });
    }
}
