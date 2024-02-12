namespace Compendium.Events
{
    public class CancellableEvent<T> : Event
        where T : struct
    {
        private T allowedValue;

        public virtual T CancelledValue { get; }
        public virtual T AllowedValue { get; }

        public override bool IsCancellable { get; } = true;

        public T IsAllowed
        {
            get => allowedValue;
            set
            {
                var current = allowedValue;

                allowedValue = value;

                if (!allowedValue.Equals(current))
                    OnAllowedChanged(current, value);
            }
        }

        public void Cancel()
            => IsAllowed = CancelledValue;

        public void Allow()
            => IsAllowed = AllowedValue;

        internal virtual void OnAllowedChanged(T previous, T current) { }
    }
}