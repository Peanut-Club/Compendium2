using Compendium.Logging.Streams.Console;
using Compendium.Logging.Streams.File;
using Compendium.Logging.Streams.Unity;
using Compendium.Pooling;
using Compendium.Utilities;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Compendium.Logging
{
    public class Log : Disposable
    {
        private Dictionary<MethodBase, string> _sourceCache = new Dictionary<MethodBase, string>();

        public static Log MainLogger { get; }

        static Log()
        {
            MainLogger = new Log(10, 50, 15); 

            MainLogger.AddStream<ConsoleStream>();
            MainLogger.AddStream<UnityStream>();

            MainLogger.AddStream(new FileLogStream(FileLogStreamMode.Interval, "", 1000));
        }

        public List<LogStream> Streams { get; } = new List<LogStream>();
        public List<string> BlacklistedSources { get; } = new List<string>();

        public Dictionary<string, LogType> TypeBypass { get; } = new Dictionary<string, LogType>();

        public LogType LogType { get; set; } = LogTypes.Recommended;
        public LogFormatting LogFormatting { get; set; } = LogFormatting.Source | LogFormatting.Message;

        public LogRateLimit RateLimit { get; }

        public string Source { get; set; }
        public string SourceTemplate { get; set; } = "%typeName%";

        public event Action<LogMessage> OnLogged;

        public Log(int logWindow, int logWindowCount, int logCooldown,

            LogType logTypes = LogTypes.Recommended, 
            LogFormatting formatting = LogFormatting.Source | LogFormatting.Message, 

            string source = null,
            string sourceTemplate = null)
        {
            LogType = logTypes;
            LogFormatting = formatting;

            Source = source;
            SourceTemplate = sourceTemplate;

            RateLimit = new LogRateLimit(logWindow, logWindowCount, logCooldown);

            RateLimit.OnRateLimited += (s, windowStart, windowReset, count) => { WriteWarning("Logger", $"Log source '{s}' is now rate-limited."); };
            RateLimit.OnRateLimitRemoved += s => { WriteInformation("Logger", $"Log source '{s}' is no longer rate-limited."); };
        }

        public void WriteTrace(string source, object message, params object[] arguments)
            => Write(LogType.Trace, source, message, arguments);

        public void WriteVerbose(string source, object message, params object[] arguments)
            => Write(LogType.Verbose, source, message, arguments);

        public void WriteDebug(string source, object message, params object[] arguments)
            => Write(LogType.Debug, source, message, arguments);

        public void WriteWarning(string source, object message, params object[] arguments)
            => Write(LogType.Warning, source, message, arguments);

        public void WriteCritical(string source, object message, params object[] arguments)
            => Write(LogType.Critical, source, message, arguments);

        public void WriteError(string source, object message, params object[] arguments)
            => Write(LogType.Error, source, message, arguments);

        public void WriteInformation(string source, object message, params object[] arguments)
            => Write(LogType.Information, source, message, arguments);

        public void WriteRaw(string message)
            => Write(LogType.Raw, null, message, null);

        public void Write(LogType type, string source, object message, object[] arguments)
        {
            if (!IsEnabled(source ?? Source, type))
                return;

            if (string.IsNullOrWhiteSpace(source))
            {
                if (string.IsNullOrWhiteSpace(Source))
                    throw new ArgumentNullException(nameof(source));
                else
                    source = Source;
            }

            if (message is null)
                throw new ArgumentNullException(nameof(message));

            var messageStr = message.ToString();

            if (string.IsNullOrWhiteSpace(messageStr))
                throw new ArgumentException($"'{message.GetType().FullName}' is an invalid message argument.");

            Write(type, source, messageStr, arguments);
        }

        public void Write(LogType type, string source, string message, object[] arguments)
        {
            if (RateLimit != null && type != LogType.Raw && !RateLimit.Verify(source))
                return;

            if (arguments != null && arguments.Length > 0)
                SuffixArguments(ref message, arguments);

            var logMessage = PoolManager.RentPoolable<LogMessage>();

            logMessage.SetContent(type, source, message);

            OnLogged?.Invoke(logMessage);

            for (int i = 0; i < Streams.Count; i++)
                Streams[i].Write(logMessage);

            PoolManager.ReturnPoolable(logMessage);
        }

        public void AddStream<TStream>() where TStream : LogStream, new()
            => AddStream(new TStream());

        public void AddStream(LogStream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            Streams.Add(stream);
        }

        public void RemoveStream<TStream>() where TStream : LogStream, new()
        {
            for (int i = 0; i < Streams.Count; i++)
            {
                if (Streams[i] is TStream)
                    Streams[i].Dispose();
            }

            Streams.RemoveAll(st => st is TStream);
        }

        public void RemoveStream(LogStream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            stream.Dispose();

            Streams.Remove(stream);
        }

        public void SuffixArguments(ref string message, object[] arguments)
        {
            for (int i = 0; i < arguments.Length; i++)
                message += $"\nArgument {i + 1}: {arguments[i]}";
        }

        public bool IsEnabled(string source, LogType type)
        {
            if (!string.IsNullOrWhiteSpace(source)
                && BlacklistedSources.Contains(source))
                return false;

            if (!string.IsNullOrWhiteSpace(source)
                && TypeBypass.TryGetValue(source, out var bypass)
                && LogTypes.IsEnabled(type, bypass))
                return true;

            return LogTypes.IsEnabled(type, LogType);
        }

        public override void DisposeInternal()
        {
            for (int i = 0; i < Streams.Count; i++)
                Streams[i].Dispose();

            Streams.Clear();
            TypeBypass.Clear();
            BlacklistedSources.Clear();

            RateLimit.Dispose();

            _sourceCache.Clear();
            _sourceCache = null;

            base.DisposeInternal();
        }

        public static void Info(string source, object message, params object[] arguments)
            => MainLogger?.WriteInformation(source, message, arguments);

        public static void Info(object message, params object[] arguments)
            => MainLogger?.WriteInformation(MainLogger?.GetSource(), message, arguments);

        public static void Warn(string source, object message, params object[] arguments)
            => MainLogger?.WriteWarning(source, message, arguments);

        public static void Warn(object message, params object[] arguments)
            => MainLogger?.WriteWarning(MainLogger?.GetSource(), message, arguments);

        public static void Error(string source, object message, params object[] arguments)
            => MainLogger?.WriteError(source, message, arguments);

        public static void Error(object message, params object[] arguments)
            => MainLogger?.WriteError(MainLogger?.GetSource(), message, arguments);

        public static void Critical(string source, object message, params object[] arguments)
            => MainLogger?.WriteCritical(source, message, arguments);

        public static void Critical(object message, params object[] arguments)
            => MainLogger?.WriteCritical(MainLogger?.GetSource(), message, arguments);

        public static void Debug(string source, object message, params object[] arguments)
            => MainLogger?.WriteDebug(source, message, arguments);

        public static void Debug(object message, params object[] arguments)
            => MainLogger?.WriteDebug(MainLogger?.GetSource(), message, arguments);

        public static void Verbose(string source, object message, params object[] arguments)
            => MainLogger?.WriteVerbose(source, message, arguments);

        public static void Verbose(object message, params object[] arguments)
            => MainLogger?.WriteVerbose(MainLogger?.GetSource(), message, arguments);

        public static void Trace(string source, object message, params object[] arguments)
            => MainLogger?.WriteTrace(source, message, arguments);

        public static void Trace(object message, params object[] arguments)
            => MainLogger?.WriteTrace(MainLogger?.GetSource(), message, arguments);

        public string GetSource()
        {
            foreach (var frame in new StackTrace().GetFrames())
            {
                var method = frame.GetMethod();

                if (method is null || method.DeclaringType is null)
                    continue;

                if (method.DeclaringType == typeof(Log))
                    continue;

                if (_sourceCache.TryGetValue(method, out var source))
                    return source;

                var type = method.DeclaringType;
                var assembly = type.Assembly.GetName();

                if (!string.IsNullOrWhiteSpace(SourceTemplate))
                    return _sourceCache[method] = SourceTemplate.Replace("%typeName%", type.Name)
                                                                .Replace("%methodName%", method.Name)
                                                                .Replace("%typeFullName%", type.FullName)
                                                                .Replace("%assemblyName%", assembly.Name)
                                                                .Replace("%assemblyVersion%", assembly.Version.ToString());
                else
                    return _sourceCache[method] = type.Name;
            }

            return "Unknown";
        }
    }
}