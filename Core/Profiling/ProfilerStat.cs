namespace Compendium.Profiling
{
    public struct ProfilerStat<TValue>
    {
        public TValue Start;
        public TValue During;
        public TValue End;
        public TValue Difference;

        public bool IsRecorded;
    }
}