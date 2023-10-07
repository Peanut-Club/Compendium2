using Compendium.Attributes;
using Compendium.Callbacks;
using Compendium.Events;
using Compendium.Extensions;
using Compendium.Logging;
using Compendium.Logging.Streams.Console;
using Compendium.Logging.Streams.File;
using Compendium.Logging.Streams.Unity;
using Compendium.Utilities.Reflection;

using HarmonyLib;

using System;
using System.Reflection;

namespace Compendium.Patching
{
    public static class Patcher
    {
        public static Harmony Instance { get; private set; } = new Harmony($"com.compendium.patcher.{DateTime.Now.Ticks}");
        public static Log Logger { get; private set; } = new Log(20, 30, 15, LogTypes.LowDebugging, LogFormatting.Source | LogFormatting.Message, "Patcher");

        [LoadCallback(Priority = Enums.Priority.High)]
        private static void Load()
        {
            Logger.AddStream<ConsoleStream>();
            Logger.AddStream<UnityStream>();

            Logger.AddStream(new FileLogStream(FileLogStreamMode.Interval, $"", 1500));

            AttributeResolver<PatchAttribute>.OnResolved += OnPatchAttributeResolved;
            AttributeResolver<PatchAttribute>.OnRemoved += OnPatchAttributeRemoved;

            AttributeResolver<EventPatchAttribute>.OnResolved += OnEventPatchAttributeResolved;
            AttributeResolver<EventPatchAttribute>.OnRemoved += OnEventPatchAttributeRemoved;

            Logger.Info("Initialized.");
        }

        [UnloadCallback(Priority = Enums.Priority.Low)]
        private static void Unload()
        {
            AttributeResolver<PatchAttribute>.OnResolved -= OnPatchAttributeResolved;
            AttributeResolver<PatchAttribute>.OnRemoved -= OnPatchAttributeRemoved;

            AttributeResolver<EventPatchAttribute>.OnResolved -= OnEventPatchAttributeResolved;
            AttributeResolver<EventPatchAttribute>.OnRemoved -= OnEventPatchAttributeRemoved;

            Logger.Info("Unloaded.");

            Logger.Dispose();
            Logger = null;

            Instance.UnpatchSelf();
            Instance = null;
        }

        public static void Unpatch(MethodBase target, MethodInfo patch)
            => Instance.Unpatch(target, patch);

        public static MethodInfo Patch(MethodBase target, Delegate replacement, PatchTiming timing, PatchType type)
            => Patch(target, replacement.GetMethodInfo(), timing, type);

        public static MethodInfo Patch(MethodBase target, MethodInfo replacement, PatchTiming timing, PatchType type)
        {
            try
            {
                switch (type)
                {
                    case PatchType.Method:
                    case PatchType.Setter:
                    case PatchType.Getter:
                        {
                            switch (timing)
                            {
                                case PatchTiming.AfterExecution:
                                    return Instance.Patch(target, null, new HarmonyMethod(replacement), null, null, null);

                                case PatchTiming.BeforeExecution:
                                    return Instance.Patch(target, new HarmonyMethod(replacement), null, null, null, null);
                            }

                            break;
                        }

                    case PatchType.Finalizer:
                        return Instance.Patch(target, null, null, null, new HarmonyMethod(replacement), null);

                    case PatchType.Transpiler:
                        return Instance.Patch(target, null, null, new HarmonyMethod(replacement), null, null);

                    case PatchType.IL:
                        return Instance.Patch(target, null, null, null, null, new HarmonyMethod(replacement));
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to patch '{target.ToName()}' with '{replacement.ToName()}' due to an exception", ex);
            }

            return null;
        }

        private static void OnEventPatchAttributeResolved(AttributeInfo<EventPatchAttribute> attributeInfo)
        {
            if (!EventManager.IsActive(attributeInfo.Attribute.EventType))
                return;

            if (attributeInfo.Location != AttributeLocation.Method)
                return;

            if (attributeInfo.Attribute.Replacement is null)
                return;

            if (attributeInfo.Attribute.Patch != null)
                return;

            attributeInfo.Attribute.Patch = Patch(attributeInfo.Attribute.Target, attributeInfo.Attribute.Replacement, attributeInfo.Attribute.Timing, attributeInfo.Attribute.Type);

            if (attributeInfo.Attribute.Patch != null)
                Logger.Debug($"Patched event method '{attributeInfo.Attribute.Target.ToName()}' by '{attributeInfo.Attribute.Replacement.ToName()}'");
        }

        private static void OnEventPatchAttributeRemoved(AttributeInfo<EventPatchAttribute> attributeInfo)
        {
            if (EventManager.IsActive(attributeInfo.Attribute.EventType))
                return;

            if (attributeInfo.Location != AttributeLocation.Method)
                return;

            if (attributeInfo.Attribute.Patch is null)
                return;

            Unpatch(attributeInfo.Attribute.Target, attributeInfo.Attribute.Patch);

            Logger.Debug($"Unpatched event method '{attributeInfo.Attribute.Target.ToName()}' (patch: '{attributeInfo.Attribute.Replacement.ToName()}')");

            attributeInfo.Attribute.Patch = null;
        }

        private static void OnPatchAttributeResolved(AttributeInfo<PatchAttribute> attributeInfo)
        {
            if (attributeInfo.Location != AttributeLocation.Method)
                return;

            if (attributeInfo.Attribute.Replacement is null)
                return;

            if (attributeInfo.Attribute.Patch != null)
                return;

            attributeInfo.Attribute.Patch = Patch(attributeInfo.Attribute.Target, attributeInfo.Attribute.Replacement, attributeInfo.Attribute.Timing, attributeInfo.Attribute.Type);

            if (attributeInfo.Attribute.Patch != null)
                Logger.Debug($"Patched method '{attributeInfo.Attribute.Target.ToName()}' by '{attributeInfo.Attribute.Replacement.ToName()}'");
        }

        private static void OnPatchAttributeRemoved(AttributeInfo<PatchAttribute> attributeInfo)
        {
            if (attributeInfo.Location != AttributeLocation.Method)
                return;

            if (attributeInfo.Attribute.Patch is null)
                return;

            Unpatch(attributeInfo.Attribute.Target, attributeInfo.Attribute.Patch);

            Logger.Debug($"Unpatched method '{attributeInfo.Attribute.Target.ToName()}' (patch: '{attributeInfo.Attribute.Replacement.ToName()}')");

            attributeInfo.Attribute.Patch = null;
        }
    }
}