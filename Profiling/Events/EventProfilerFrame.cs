using Compendium.Events;

using System;

namespace Compendium.Profiling.Events
{
    public struct EventProfilerFrame
    {
        public ProfilerStamp Stamp;

        public ProfilerStat<long> Memory;

        public ProfilerStat<double> Ticks;
        public ProfilerStat<double> Time;

        public Exception Exception;

        public double Execution;

        public bool IsValid;

        public long Number;

        public EventData Event;
    }
}