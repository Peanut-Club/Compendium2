namespace Compendium.Events
{
    public class CancellableEventInfo<TValue> : EventInfo
    {
        public TValue Cancellation { get; set; } = default;

        public override bool IsCancellable => true;
    }
}