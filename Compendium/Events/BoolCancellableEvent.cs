namespace Compendium.Events
{
    public class BoolCancellableEvent : CancellableEvent<bool>
    {
        internal override bool AllowedValue { get; } = true;
        internal override bool CancelledValue { get; } = false;
    }
}