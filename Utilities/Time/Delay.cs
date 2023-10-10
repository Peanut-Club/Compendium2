using System;

namespace Compendium.Utilities.Time
{
    public struct Delay
    {
        public readonly bool IsValid;
        public readonly double Value;

        private Delay(double value)
        {
            IsValid = true;
            Value = value;
        }

        public static Delay FromTicks(long ticks)
            => new Delay(ticks / TimeSpan.TicksPerMillisecond);

        public static Delay FromSeconds(double seconds)
            => new Delay(seconds * 1000);

        public static Delay FromMilliseconds(double milliseconds)
            => new Delay(milliseconds);

        public static Delay FromMinutes(double minutes)
            => new Delay(minutes * 60000);

        public static Delay FromHours(double hours)
            => new Delay(hours * 3600000);
    }
}