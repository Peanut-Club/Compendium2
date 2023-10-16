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

        public static void Start(MethodInfo method, ProfilerMode mode = ProfilerMode.Patched, int maxFrames = -1) 
        {
            AssertUtils.ThrowIfNull(method);

            if (GetRecord(method) != null)
                return;

            var record = new ProfilerRecord(method, maxFrames, mode);

            record.Clear();
            record.IsPaused = false;

            _records.Add(record);

            if (mode is ProfilerMode.Patched)
                ProfilerPatcher.Apply(method);
        }

        public static void Stop(MethodInfo method)
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

        public static void Pause(MethodInfo method)
            => GetRecord(method)!.IsPaused = true;

        public static void Resume(MethodInfo method)
            => GetRecord(method)!.IsPaused = false;

        public static ProfilerRecord GetRecord(MethodBase method)
        {
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].Target == method)
                    return _records[i];
            }

            return null;
        }
    }
}