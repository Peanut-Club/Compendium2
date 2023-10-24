using Compendium.Logging;

namespace Compendium.Extensions
{
    public static class LogExtensions
    {
        public static void Info(this Log logger, object message, params object[] arguments)
            => logger.WriteInformation(null, message, arguments);

        public static void Error(this Log logger, object message, params object[] arguments)
            => logger.WriteError(null, message, arguments);

        public static void Debug(this Log logger, object message, params object[] arguments)
            => logger.WriteDebug(null, message, arguments);

        public static void Trace(this Log logger, object message, params object[] arguments)
            => logger.WriteTrace(null, message, arguments);

        public static void Verbose(this Log logger, object message, params object[] arguments)
            => logger.WriteVerbose(null, message, arguments);

        public static void Critical(this Log logger, object message, params object[] arguments)
            => logger.WriteCritical(null, message, arguments);

        public static void Raw(this Log logger, string message)
            => logger.WriteRaw(message);

        public static void Warn(this Log logger, object message, params object[] arguments)
            => logger.WriteWarning(null, message, arguments);
    }
}