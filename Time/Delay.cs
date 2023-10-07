using System;

namespace Compendium.Time
{
    public struct Delay
    {
        public bool IsValid;
        public double Value;

        public static Delay FromTicks(long ticks)
            => new Delay
            {
                IsValid = true,
                Value = ticks / TimeSpan.TicksPerMillisecond
            };

        public static Delay FromSeconds(double seconds)
            => new Delay
            {
                IsValid = true,
                Value = TimeSpan.FromSeconds(seconds).TotalMilliseconds
            };

        public static Delay FromMilliseconds(double milliseconds)
            => new Delay
            {
                IsValid = true,
                Value = milliseconds
            };
    }
}