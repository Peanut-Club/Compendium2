using Compendium.Pooling;

using System;

namespace Compendium.Logging
{
    public class LogMessage : PoolableBase
    {
        public LogType Type { get; private set; }

        public DateTime Time { get; private set; }

        public string Source { get; private set; }
        public string Message { get; private set; }

        public void SetContent(LogType type, string source, string message)
        {
            Type = type;
            Time = DateTime.Now;
            Source = source;
            Message = message;
        }

        public override void OnPooled()
        {
            Source = null;
            Message = null;
            Time = default;
            Type = LogType.Information;

            base.OnPooled();
        }
    }
}