using UnityEngine;

namespace Compendium.Logging.Streams.Unity
{
    public class UnityStream : LogStream
    {
        public override void Write(LogMessage message)
        {
            switch (message.Type)
            {
                case LogType.Raw:
                    Debug.Log(message.Message);
                    return;

                case LogType.Warning:
                    Debug.LogWarning($"{message.Time} [{message.Source} / {message.Type}] {message.Message}");
                    return;

                case LogType.Critical:
                case LogType.Error:
                    Debug.LogError($"{message.Time} [{message.Source} / {message.Type}] {message.Message}");
                    return;

                case LogType.Debug:
                case LogType.Verbose:
                case LogType.Trace:
                    Debug.Log($"{message.Time} [{message.Source} / {message.Type}] {message.Message}");
                    return;
            }
        }
    }
}