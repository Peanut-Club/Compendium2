using System.Collections.Generic;
using System;

namespace Compendium.Utilities.Reflection
{
    public class CallStats<TTarget>
    {
        private static readonly List<CallStats<TTarget>> _stats = new List<CallStats<TTarget>>();

        private bool _recorded;
        
        public TTarget Target { get; }

        public double Last { get; private set; }
        public double Highest { get; private set; }
        public double Lowest { get; private set; }

        public double Average => _recorded ? (Highest * Lowest) / 2 : -1;

        public int Counter { get; private set; }

        public CallStats(TTarget target)
        {
            Target = target;
            Reset();
        }

        public void Record(double time)
        {
            Counter++;

            Last = time;

            if (!_recorded)
            {
                Last = time;
                Highest = time;
                Lowest = time;

                _recorded = true;

                return;
            }

            if (time > Highest)
                Highest = time;

            if (time < Lowest)
                Lowest = time;
        }

        public void Reset()
        {
            _recorded = false;

            Last = -1;
            Highest = -1;
            Lowest = -1;

            Counter = 0;
        }

        public static CallStats<TTarget> Create(TTarget target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            for (int i = 0; i < _stats.Count; i++)
            {
                if (_stats[i].Target != null && _stats[i].Target.Equals(target))
                    return _stats[i];
            }

            return new CallStats<TTarget>(target);
        }
    }
}