namespace Compendium.Events
{
    public class CancellableEvent<T> : Event
        where T : struct
    {
        private T allowedValue;
        private EventMethod changedByHandler;

        internal virtual T CancelledValue { get; }
        internal virtual T AllowedValue { get; }

        public override bool IsCancellable { get; } = true;

        public T IsAllowed
        {
            get => allowedValue;
            set
            {
                changedByHandler = CurrentHandler;

                var current = allowedValue;

                allowedValue = value;

                if (!allowedValue.Equals(current))
                    OnAllowedChanged(current, value);
            }
        }

        public EventMethod CancellationChangedBy
        {
            get => changedByHandler;
        }

        public void Cancel()
            => IsAllowed = CancelledValue;

        public void Allow()
            => IsAllowed = AllowedValue;

        internal virtual void OnAllowedChanged(T previous, T current) { }
    }
}