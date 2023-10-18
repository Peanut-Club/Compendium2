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
    }
}