using Compendium.Utilities.Reflection;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compendium.Profiling
{
    public static class ProfilerPatcher
    {
        public static Harmony Harmony { get; } = new Harmony($"com.compendium.profiler.{DateTime.Now.Ticks}");

        public static HarmonyMethod Prefix { get; } = new HarmonyMethod(typeof(ProfilerPatch).Method("Prefix"));
        public static HarmonyMethod Finalizer { get; } = new HarmonyMethod(typeof(ProfilerPatch).Method("Finalizer"));

        public static void Apply(MethodBase target)
        {
            if (IsPatched(target))
                return;

            Harmony.Patch(target, Prefix, null, null, Finalizer, null);
        }

        public static void Remove(MethodBase target)
        {
            if (!IsPatched(target))
                return;

            Harmony.Unpatch(target, HarmonyPatchType.Prefix, Harmony.Id);
            Harmony.Unpatch(target, HarmonyPatchType.Postfix, Harmony.Id);
        }

        public static bool IsPatched(MethodBase target)
            => Harmony.GetPatchInfo(target).Postfixes.Count(p => p.owner == Harmony.Id) > 0;
    }

    public static class ProfilerPatch
    {
        public static readonly Dictionary<MethodBase, ProfilerRecord> Records = new Dictionary<MethodBase, ProfilerRecord>();

        [HarmonyPriority(2500)]
        public static void Prefix(MethodBase __originalMethod, ref long __state)
        {
            if (!Records.ContainsKey(__originalMethod))
                Records[__originalMethod] = Profiler.GetRecord(__originalMethod);

            var record = Records[__originalMethod];

            if (record.TryNewFrame(out var frame))
                __state = frame.Number;
            else
                __state = -1;
        }

        [HarmonyPriority(2500)]
        public static void Finalizer(MethodBase __originalMethod, Exception __exception, ref long __state)
        {
            if (__state != -1 && Records.ContainsKey(__originalMethod))
                Records[__originalMethod].EndFrame(__state, __exception);
        }
    }
}