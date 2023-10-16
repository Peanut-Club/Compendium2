using Compendium.Pooling;

using System;

namespace Compendium.Utilities
{
    public class Delay : PoolableBase
    {
        public double Value { get; set; }

        public double ToSeconds()
            => Value * 1000;

        public double ToMinutes()
            => Value * 60000;

        public double ToHours()
            => Value * 360000;

        public bool IsValid()
            => Value > 0;

        public TimeSpan ToSpan()
            => TimeSpan.FromMilliseconds(Value);

        public override void OnPooled()
        {
            base.OnPooled();
            Value = -1;
        }

        public static Delay FromMilliseconds(double ms)
        {
            var delay = PoolManager.RentPoolable<Delay>();
            delay.Value = ms;
            return delay;
        }

        public static Delay FromSeconds(double secs)
            => FromMilliseconds(secs / 1000);

        public static Delay FromMinutes(double mins)
            => FromMilliseconds(mins / 60000);

        public static Delay FromHours(double hours)
            => FromMilliseconds(hours / 360000);

        public static Delay FromSpan(TimeSpan span)
            => FromMilliseconds(span.TotalMilliseconds);

        public static Delay Until(DateTime time)
            => FromMilliseconds((time - DateTime.Now).Ticks / TimeSpan.TicksPerMillisecond);
    }
}