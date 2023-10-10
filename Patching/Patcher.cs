using Compendium.Attributes;
using Compendium.Callbacks;
using Compendium.Events;
using Compendium.Extensions;
using Compendium.Logging;

using HarmonyLib;

using MonoMod.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compendium.Patching
{
    public static class Patcher
    {
        private static List<PatchInfo> _patches = new List<PatchInfo>();

        public static Harmony Instance { get; private set; } = new Harmony($"com.compendium.patcher.{DateTime.Now.Ticks}");
        public static Log Logger { get; private set; } = new Log(20, 30, 15, LogTypes.LowDebugging, LogFormatting.Source | LogFormatting.Message, "Patcher");

        [LoadCallback(Priority = Enums.Priority.Low)]
        private static void Load()
            => ApplyPatches(Assembly.GetExecutingAssembly());

        [UnloadCallback(Priority = Enums.Priority.Low)]
        private static void Unload()
        {
            Logger?.Dispose();
            Logger = null;

            Instance.UnpatchSelf();
            Instance = null;
        }

        public static void RemovePatches(Assembly assembly)
        {
            for (int i = 0; i < _patches.Count; i++)
            {
                if (_patches[i].Method.DeclaringType.Assembly == assembly)
                    _patches[i].Unapply();
            }

            _patches.RemoveAll(p => p.Method.DeclaringType.Assembly == assembly);
        }

        public static void ApplyPatches(Assembly assembly)
        {
            var types = assembly.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                var attributes = AttributeResolver<PatchAttribute>.ResolveAttributes(types[i], null, AttributeResolveFlags.Method);

                if (attributes is null || attributes.Count < 1)
                    continue;

                foreach (var attr in attributes)
                {
                    if (attr.Attribute.Info is null)
                        continue;

                    if (_patches.Any(p => p.Method == attr.Attribute.Info.Method && p.Target == attr.Attribute.Info.Target))
                        continue;

                    _patches.Add(attr.Attribute.Info);

                    Logger.Debug($"Found patch: '{attr.Attribute.Info.Name}'");

                    if (attr.Attribute.Info.Flags.Has(PatchFlags.IsEvent)
                        && attr.Attribute.eventType != null)
                    {
                        if (EventManager.IsAny(attr.Attribute.eventType))
                            attr.Attribute.Info.Apply();
                    }
                    else
                        attr.Attribute.Info.Apply();
                }
            }
        }
    }
}