using Compendium.Utilities;

using System.Collections.Generic;
using System.Reflection;

namespace Compendium.Profiling
{
    public static class Profiler
    {
        private static readonly List<ProfilerRecord> _records = new List<ProfilerRecord>();

        public static bool IsActive(ProfilerRecord record)
        {
            AssertUtils.ThrowIfNull(record);

            if (record.Mode is ProfilerMode.Patched)
                return ProfilerPatcher.IsPatched(record.Target);

            return !record.IsPaused;
        }

        public static void Start(MethodBase method, ProfilerMode mode = ProfilerMode.Patched, int maxFrames = -1) 
        {
            AssertUtils.ThrowIfNull(method);

            var record = GetRecord(method);

            if (record != null)
            {
                record.IsPaused = false;

                if (record.Mode is ProfilerMode.Patched && !ProfilerPatcher.IsPatched(method))
                    ProfilerPatcher.Apply(method);

                return;
            }

            record = new ProfilerRecord(method, maxFrames, mode);

            record.Clear();
            record.IsPaused = false;

            _records.Add(record);

            if (mode is ProfilerMode.Patched)
                ProfilerPatcher.Apply(method);
        }

        public static void Stop(MethodBase method)
        {
            AssertUtils.ThrowIfNull(method);

            var record = GetRecord(method);

            if (record is null)
                return;

            if (record.Mode is ProfilerMode.Patched)
                ProfilerPatcher.Remove(method);

            record.IsPaused = true;
            record.Clear();
        }

        public static void Pause(MethodBase method)
            => GetRecord(method)!.IsPaused = true;

        public static void Resume(MethodBase method)
            => GetRecord(method)!.IsPaused = false;

        public static ProfilerRecord GetRecord(MethodBase method, int maxFrames = -1, ProfilerMode mode = ProfilerMode.Patched)
        {
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].Target == method)
                    return _records[i];
            }

            return new ProfilerRecord(method, maxFrames, mode);
        }
    }
}