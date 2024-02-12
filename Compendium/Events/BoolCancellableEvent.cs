namespace Compendium.Events
{
    public class BoolCancellableEvent : CancellableEvent<bool>
    {
        public override bool AllowedValue { get; } = true;
        public override bool CancelledValue { get; } = false;
    }
}