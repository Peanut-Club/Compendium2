using System;

namespace Compendium.Logging.Streams.Console
{
    public class ConsoleStream : LogStream
    {
        public override void Write(LogMessage message)
            => ServerConsole.AddLog(LogFormatter.Format(Format(message)), TypeToColor(message.Type));

        private static ConsoleColor TypeToColor(LogType type)
        {
            switch (type)
            {
                case LogType.Raw:
                    return ConsoleColor.Gray;

                case LogType.Information:
                    return ConsoleColor.Green;

                case LogType.Error:
                    return ConsoleColor.Red;

                case LogType.Critical:
                    return ConsoleColor.DarkRed;

                case LogType.Debug:
                    return ConsoleColor.Cyan;

                case LogType.Trace:
                    return ConsoleColor.DarkGray;

                case LogType.Verbose:
                    return ConsoleColor.Magenta;

                case LogType.Warning:
                    return ConsoleColor.DarkYellow;

                default:
                    return ConsoleColor.White;
            }
        }

        private static string Format(LogMessage message)
        {
            switch (message.Type)
            {
                case LogType.Raw:
                    return message.Message;

                case LogType.Information:
                    return $"&/&_[&b&!INFO&B] &_[&b&/{message.Source}&B] &/{message.Message}&r";

                case LogType.Warning:
                    return $"&/&_[&b&3WARN&B] &_[&b&/{message.Source}&B] &/{message.Message}&r";

                case LogType.Critical:
                    return $"&/&_[&b&1&CRITICAL&B] &_[&b&/{message.Source}&B] &/{message.Message}&r";

                case LogType.Debug:
                    return $"&/&_[&bDEBUG&B] &_[&b&/{message.Source}&B] &/{message.Message}&r";

                case LogType.Error:
                    return $"&/&_[&b&9ERROR&B] &_[&b&/{message.Source}&B] &/{message.Message}&r";

                case LogType.Verbose:
                    return $"&/&_[&b&-VERBOSE&B] &_[&b&/{message.Source}&B] &/{message.Message}&r";

                case LogType.Trace:
                    return $"&/&_[&b&/TRACE&B] &_[&b&/{message.Source}&B] &/{message.Message}&r";

                default:
                    return $"&/&_[&b{message.Type.ToString().ToUpperInvariant()}&B] &/{message.Message}&r";
            }
        }
    }
}