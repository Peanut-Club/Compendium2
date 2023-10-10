using Compendium.Pooling;

namespace Compendium.Events
{
    public class EventInfo : PoolableBase
    {
        public virtual EventData Event { get; }
        public virtual bool IsCancellable { get; }
    }
}