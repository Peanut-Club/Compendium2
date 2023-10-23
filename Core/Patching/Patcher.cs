using Compendium.Callbacks;
using Compendium.Logging;
using Compendium.Logging.Formatting;

using HarmonyLib;

using System;
using System.Collections.Generic;

namespace Compendium.Patching
{
    public static class Patcher
    {
        private static List<PatchInfo> _patches = new List<PatchInfo>();

        public static Harmony Instance { get; private set; } = new Harmony($"com.compendium.patcher.{DateTime.Now.Ticks}");
        public static Log Logger { get; private set; } = new Log(20, 30, 15, LogTypes.LowDebugging, LogFormatting.Source | LogFormatting.Message, "Patcher");

        [UnloadCallback(Priority = Enums.Priority.Low)]
        private static void Unload()
        {
            Logger?.Dispose();
            Logger = null;

            Instance.UnpatchSelf();
            Instance = null;
        }


    }
}