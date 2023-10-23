using Compendium.Profiling;
using Compendium.Utilities;
using Compendium.Utilities.Reflection;

using System;
using System.Reflection;

namespace Compendium.Modules
{
    public class ModuleUpdateInfo
    {
        public FastDelegate Caller { get; }
        public ProfilerRecord Profiler { get; }

        public Delay Delay { get; set; }

        public ModuleUpdateSource Source { get; }

        public bool IsInherited { get; }
        public bool IsEnabled { get; set; } = true;
        public bool IsProfiled { get => Profiler != null && !Profiler.IsPaused; set => Profiler!.IsPaused = value; }

        public DateTime LastCalled { get; internal set; } = DateTime.MinValue;

        public ModuleUpdateInfo(Delay delay, MethodInfo method, ModuleUpdateSource source, bool isInherited)
        {
            Delay = delay;
            Source = source;

            IsInherited = isInherited;

            Caller = method.GetFastInvoker(true);

            Profiler = Profiling.Profiler.GetRecord(method, -1, ProfilerMode.Manual);
            Profiler.IsPaused = true;
        }
    }
}