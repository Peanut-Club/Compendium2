using Compendium.Utilities;

namespace Compendium.Logging
{
    public abstract class LogStream : Disposable
    {
        public abstract void Write(LogMessage message);
    }
}