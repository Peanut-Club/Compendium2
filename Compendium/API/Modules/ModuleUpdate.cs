using Common.Utilities;

using System;
using System.Reflection;

using UnityEngine;

namespace Compendium.API.Modules
{
    public class ModuleUpdate
    {
        public MethodInfo UpdateMethod { get; internal set; }

        public Func<int> Interval { get; }

        public DateTime LastCallTime { get; internal set; }
        public DateTime NextCallTime { get; internal set; }

        public string UpdateMethodName { get; }

        public int IntervalMax { get; } = 10;
        public int IntervalMin { get; } = 10;

        public bool IsFixedInterval { get; }
        public bool IsDynamicInterval { get; }

        public bool IsActiveDuringRestart { get; }
        public bool IsActiveDuringWaiting { get; }

        public ModuleUpdate(string methodName, int maxInterval, int minInterval, bool isActiveDuringRestart, bool isActiveDuringWaiting)
        {
            UpdateMethodName = methodName;

            IntervalMax = maxInterval;
            IntervalMin = minInterval;

            if (IntervalMin > IntervalMax || IntervalMax == IntervalMin)
                throw new ArgumentOutOfRangeException(nameof(maxInterval));

            IsActiveDuringRestart = isActiveDuringRestart;
            IsActiveDuringWaiting = isActiveDuringWaiting;

            Interval = () => Generator.Instance.GetInt32(IntervalMin, IntervalMax);
        }

        public ModuleUpdate(string methodName, int interval, bool isActiveDuringRestart, bool isActiveDuringWaiting)
        {
            UpdateMethodName = methodName;

            IntervalMax = interval;
            IntervalMin = interval;

            IsFixedInterval = true;
            IsDynamicInterval = interval < 0;

            IsActiveDuringRestart = isActiveDuringRestart;
            IsActiveDuringWaiting = isActiveDuringWaiting;

            Interval = () =>
            {
                if (IsDynamicInterval)
                    return Mathf.CeilToInt(Time.time);
                else
                    return IntervalMax;
            };
        }
    }
}