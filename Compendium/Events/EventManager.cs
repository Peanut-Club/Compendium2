using Common.Extensions;
using Common.IO.Collections;
using Common.Logging;
using Common.Patching;
using Common.Utilities;

using System;
using System.Linq;
using System.Reflection;

namespace Compendium.Events
{
    public static class EventManager
    {
        private static readonly LockedDictionary<Type, EventInfo> knownEvents = new LockedDictionary<Type, EventInfo>();

        public static LogOutput Log { get; } = new LogOutput("Event Manager");

        #region Event Invocation
        public static bool InvokeBool<T>(T ev, bool isAllowed)
            where T : BoolCancellableEvent
            => Invoke(ev, isAllowed);

        public static TCancellation Invoke<T, TCancellation>(T ev, TCancellation isAllowed) 
            where T : CancellableEvent<TCancellation>
            where TCancellation : struct
        {
            ev.IsAllowed = isAllowed;

            Invoke(ev);

            return ev.IsAllowed;
        }

        public static void Invoke<T>(T ev) where T : Event
        {
            if (!knownEvents.TryGetValue(typeof(T), out var evInfo))
            {
                Log.Warn($"Attempted to invoke an unknown event: {typeof(T).FullName}");
                return;
            }

            if (!evInfo.HasAnyListeners && evInfo.Event is null)
                return;

            try
            {
                if (evInfo.HasAnyCustom)
                    ev.SetProperties();

                evInfo.Invoke(ev);

                if (evInfo.HasAnyCustom)
                    ev.ReturnProperties();
            }
            catch (Exception ex)
            {
                Log.Error($"An exception has occured while invoking event '{evInfo.Name}':\n{ex}");
            }
        }
        #endregion

        #region Handler Registration
        public static void RegisterHandlers()
            => RegisterHandlers(Assembly.GetCallingAssembly());

        public static void UnregisterHandlers()
            => UnregisterHandlers(Assembly.GetCallingAssembly());

        public static void RegisterHandlers(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
                RegisterHandlers(type, null);
        }

        public static void UnregisterHandlers(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
                UnregisterHandlers(type, null);
        }

        public static void RegisterHandlers(Type type, object instance = null)
        {
            foreach (var method in type.GetAllMethods())
                InternalRegisterHandler(method, instance, false);
        }

        public static void UnregisterHandlers(Type type, object instance = null)
        {
            foreach (var method in type.GetAllMethods())
                InternalUnregisterHandler(method, instance, false);
        }

        public static void RegisterHandler(MethodInfo handler, object target = null)
            => InternalRegisterHandler(handler, target, true);

        public static void UnregisterHandler(MethodInfo handler, object target = null)
            => InternalUnregisterHandler(handler, target, true);

        private static bool InternalUnregisterHandler(MethodInfo handler, object target = null, bool isManual = true)
        {
            if (isManual && !TypeInstanceValidator.IsValid(handler.DeclaringType, target, false))
            {
                Log.Warn($"Tried to unregister a non-static method '{handler.ToName()}' without a class instance.");
                return false;
            }

            var targetEv = EventUtils.GetEvent(handler, knownEvents);

            if (targetEv is null)
            {
                if (isManual)
                    Log.Warn($"Tried to unregister an event handler with an unrecognizable event type: {handler.ToName()}");

                return false;
            }

            if (!targetEv.Listeners.TryGetFirst(listener =>
                listener.IsValid && listener.Target == handler && ((listener.Instance is null && target is null)
                || ((listener.Instance != null && target != null) && listener.Instance == target)), out var method))
            {
                if (isManual)
                    Log.Warn($"Tried to unregister an event handler that was not previously registered ({handler.ToName()})");

                return false;
            }

            if (targetEv.Listeners.Remove(method))
            {
                if (!targetEv.HasAnyListeners && targetEv.HasPatch)
                    targetEv.IsPatched = false;

                return true;
            }

            return false;
        }

        private static void InternalRegisterHandler(MethodInfo handler, object target, bool isManual)
        {
            if (isManual && !TypeInstanceValidator.IsValid(handler.DeclaringType, target, false))
            {
                Log.Warn($"Tried to register a non-static method '{handler.ToName()}' without a class instance.");
                return;
            }

            var targetEv = EventUtils.GetEvent(handler, knownEvents);

            if (targetEv is null)
            {
                if (isManual)
                    Log.Warn($"Tried to register an event handler with an unrecognizable event type: {handler.ToName()}");

                return;
            }

            if (targetEv.Listeners.Any(listener => 
                listener.IsValid && listener.Target == handler && ((listener.Instance is null && target is null) 
                || ((listener.Instance != null && target != null) && listener.Instance == target))))
            {
                if (isManual)
                    Log.Warn($"Tried to register a duplicate event handler: {handler.ToName()}");

                return;
            }

            targetEv.Listeners.Add(new EventMethod(handler, target, false));

            if (targetEv.HasPatch && !targetEv.IsPatched)
                targetEv.IsPatched = true;

            Log.Verbose($"Registered event handler: {handler.ToName()} ({targetEv.Name})");
        }
        #endregion

        #region Event Registration
        public static void RegisterEvents()
            => RegisterEvents(Assembly.GetCallingAssembly());

        public static void UnregisterEvents()
            => UnregisterEvents(Assembly.GetCallingAssembly());

        public static void RegisterEvents(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(Event)))
                    continue;

                var hasPatch = type.HasAttribute<EventPatchAttribute>(out var eventPatchAttribute);
                var delegateEvent = type.Event("OnEvent");

                PatchInfo patch = null;

                if (hasPatch)
                {
                    if (eventPatchAttribute.Patch is null)
                    {
                        Log.Warn($"Patch attribute of event '{type.Name}' contains a null patch!");
                        continue;
                    }

                    if (!eventPatchAttribute.Patch.HasAttribute<PatchAttribute>(out var patchAttribute))
                    {
                        Log.Warn($"Patch attribute of event '{type.Name}' refers to a method without a patch attribute.");
                        continue;
                    }

                    if (!patchAttribute.IsValid)
                    {
                        Log.Warn($"Patch attribute of event '{type.Name}' refers to a method with an invalid patch.");
                        continue;
                    }

                    patch = new PatchInfo(
                        patchAttribute.Target,

                        eventPatchAttribute.Patch,

                        null,

                        patchAttribute.Target,
                        patchAttribute.Type);
                }

                if (knownEvents.ContainsKey(type))
                {
                    Log.Warn($"Tried to register a duplicate event: {type.Name}");
                    return;
                }

                knownEvents[type] = new EventInfo(
                    type,

                    hasPatch ? patch : null,

                    delegateEvent);

                Log.Verbose($"Registered event: {type.FullName}");
            }
        }

        public static void UnregisterEvents(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (knownEvents.TryGetValue(type, out var ev))
                {
                    ev.IsPatched = false;
                    ev.Listeners.Clear();

                    Log.Verbose($"Unregistered event: {ev.Name}");
                }

                knownEvents.Remove(type);
            }
        }
        #endregion
    }
}