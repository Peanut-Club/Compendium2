using Compendium.Assemblies;
using Compendium.Attributes;
using Compendium.Logging;
using Compendium.Utilities.Reflection;

using MonoMod.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compendium.Events
{
    public static class EventManager
    {
        private static readonly HashSet<EventData> _events;

        public static IReadOnlyCollection<EventData> Events => _events;

        static EventManager()
        {
            _events = new HashSet<EventData>();

            RegisterEvents(AssemblyManager.MainAssembly.Assembly);

            AttributeResolver<EventAttribute>.OnRemoved += OnAttributeRemoved;
            AttributeResolver<EventAttribute>.ResolveAttributes(AssemblyManager.MainAssembly.Assembly, AttributeResolveFlags.Method);
        }

        public static bool RegisterEvent(EventData data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (Events.Any(ev => ev.Type == data.Type))
                return false;

            return _events.Add(data);
        }

        public static void RegisterEvents(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                foreach (var evInfo in type.GetAllEvents())
                {
                    try
                    {
                        if (!evInfo.HasAttribute<EventDataDelegateAttribute>(out var eventDataDelegateAttribute))
                            continue;

                        if (_events.Any(ev => ev.Type == eventDataDelegateAttribute.Type))
                            continue;

                        _events.Add(new EventData(eventDataDelegateAttribute.Type, evInfo));

                        Log.Trace("Event Manager", $"Registered event data '{eventDataDelegateAttribute.Type.FullName}' ({evInfo.ToName()})");
                    }
                    catch (Exception ex)
                    {
                        Log.Critical("Event Manager", $"Failed to register event '{evInfo.ToName()}' due to an exception.", ex);
                    }
                }
            }
        }

        public static bool UnregisterEvent(EventData data)
            => _events.RemoveWhere(ev => ev.Type == data.Type) > 0;

        public static bool UnregisterEvents(Assembly assembly)
            => _events.RemoveWhere(ev => ev.Type.Assembly == assembly) > 0;

        public static EventData GetEvent(Type infoType)
        {
            foreach (var ev in _events)
            {
                if (ev.Type == infoType)
                    return ev;
            }

            return null;
        }

        public static void RegisterHandler(MethodBase target, object handle)
        {
            if (target.HasAttribute<EventAttribute>(out var eventAttribute)
                && eventAttribute.Event != null)
                RegisterHandler(target, handle, eventAttribute.Event);
            else
            {
                var methodParams = target.GetParameters();

                if (methodParams.Length <= 0)
                    throw new InvalidOperationException($"Method '{target.ToName()}' cannot be registered; failed to identify event type (missing event parameter)");

                if (methodParams.Length != 1)
                    throw new InvalidOperationException($"Method '{target.ToName()}' cannot be registered; overload has too many or too few parameters.");

                methodParams[0].ParameterType.VerifyEventType();

                foreach (var ev in _events)
                {
                    if (ev.Type == methodParams[0].ParameterType)
                    {
                        RegisterHandler(target, handle, ev);
                        return;
                    }
                }

                throw new InvalidOperationException($"Method '{target.ToName()}' cannot be registered; overload does not have a registered event parameter.");
            }
        }

        public static void RegisterHandler(MethodBase target, object handle, EventData eventData)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            if (!ObjectUtilities.VerifyClassInstanceForMember(target, handle))
                throw new ArgumentException(nameof(handle));

            if (eventData is null)
                throw new ArgumentNullException(nameof(eventData));

            try
            {
                var methodParams = target.GetParameters();
                var del = target.CreateDelegate(typeof(Action<>).MakeGenericType(methodParams[0].ParameterType), handle);

                eventData.AddHandler(del);

                Log.Debug("Event Manager", $"Registered event handler '{target.ToName()}' to event '{eventData.Type.Name}'");
            }
            catch (Exception ex)
            {
                Log.Critical("Event Manager", $"Failed to register event handler '{target.ToName()}' to event '{eventData.Type.Name}' due to an exception.", ex);
            }
        }

        public static void UnregisterHandler(MethodBase target, object handle)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            if (!ObjectUtilities.VerifyClassInstanceForMember(target, handle))
                throw new ArgumentException(nameof(handle));
            
            foreach (var ev in _events)
            {
                var list = ev.Delegate.GetInvocationList();

                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].Method == target && ObjectUtilities.IsInstance(handle, list[i].Target))
                    {
                        ev.RemoveHandler(list[i]);

                        Log.Debug("Event Manager", $"Unregistered event handler '{target.ToName()}' from event '{ev.Type.Name}'");
                    }
                }
            }
        }

        private static void OnAttributeRemoved(AttributeInfo<EventAttribute> attribute)
        {
            if (attribute.Location != AttributeLocation.Method || attribute.Method is null)
                return;

            UnregisterHandler(attribute.Method, attribute.Handle);
        }
    }
}