using System;

namespace Compendium.Profiling
{
    public struct ProfilerFrame
    {
        public ProfilerStamp Stamp;

        public ProfilerStat<long> Memory;

        public ProfilerStat<double> Ticks;
        public ProfilerStat<double> Time;

        public Exception Exception;

        public double Execution;

        public bool IsValid;

        public long Number;
    }
}