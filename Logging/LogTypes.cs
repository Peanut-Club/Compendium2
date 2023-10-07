namespace Compendium.Logging
{
    public static class LogTypes
    {
        public const LogType AllTypes = LogType.Trace | LogType.Verbose | LogType.Debug | LogType.Information | LogType.Warning | LogType.Error | LogType.Critical;    
        public const LogType Recommended = LogType.Information | LogType.Warning | LogType.Error | LogType.Critical;
        public const LogType LowDebugging = LogType.Debug;
        public const LogType MidDebugging = LogType.Debug | LogType.Verbose;
        public const LogType HighDebugging = LogType.Debug | LogType.Verbose | LogType.Trace;

        public static bool IsEnabled(LogType type, LogType enabled)
            => (enabled & type) == type;

        public static bool IsEnabled(LogFormatting formatting, LogFormatting enabled)
            => (enabled & formatting) == formatting;
    }
}