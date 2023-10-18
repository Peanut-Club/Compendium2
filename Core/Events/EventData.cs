using Compendium.Assemblies;
using Compendium.Logging;
using Compendium.Utilities.Reflection;

using MonoMod.Utils;

using System;

namespace Compendium.Events
{
    public class EventData
    {
        private object[] _callBuffer = new object[1];

        public Type Type { get; }
        public Type Patch { get; }

        public Delegate Delegate { get; private set; }
        public FastReflectionHelper.FastInvoker Invoker { get; }

        public System.Reflection.EventInfo Event { get; }

        public EventData(Type eventType, System.Reflection.EventInfo eventInfo)
        {
            if (eventType is null)
                throw new ArgumentNullException(nameof(eventType));

            eventType.VerifyEventType();

            Type = eventType;
            Event = eventInfo;

            Delegate = Event.DeclaringType.GetField(Event.Name).GetValue(null) as Delegate;

            var method = Event.DeclaringType.Method($"Invoke{Event.Name}");

            if (method is null)
            {
                foreach (var m in Event.DeclaringType.GetAllMethods())
                {
                    if (m.HasAttribute<EventDataInvokerAttribute>(out var eventInvokerAttribute)
                        && eventInvokerAttribute.Event == Type)
                    {
                        method = m;
                        break;
                    }
                }
            }

            if (method != null)
                Invoker = method.GetFastInvoker();
            else
                Log.Warn("Event Data", $"Event '{Event.Name}' (of type {Event.DeclaringType.FullName}) does not have a valid invoker method. This can be ignored if invoked manually.");

            if (Event is null || Delegate is null)
                throw new ArgumentException($"Failed to find a handler corresponding to the event specified as 'eventType'.");

            foreach (var type in AssemblyManager.MainAssembly.Assembly.GetTypes())
            {
                if (!type.Namespace.StartsWith("Compendium.Patching.Patches"))
                    continue;

                if (!type.HasAttribute<EventDataPatchAttribute>(out var eventDataPatchAttribute))
                    continue;

                if (eventDataPatchAttribute.Type != null && eventDataPatchAttribute.Type == Type)
                {
                    Patch = type;
                    break;
                }
            }
        }

        public virtual void AddHandler(Delegate del)
        {
            if (del is null)
                throw new ArgumentNullException(nameof(del));

            if (Event is null)
                throw new InvalidOperationException($"This event is either not initialized or failed to initialize.");

            try { Event.RemoveEventHandler(del.Target, del); } catch { }
            try { Event.AddEventHandler(del.Target, del); } catch { }

            try { Delegate = Event.DeclaringType.GetField(Event.Name).GetValue(null) as Delegate; } catch { }
        }

        public virtual void RemoveHandler(Delegate del)
        {
            if (del is null)
                throw new ArgumentNullException(nameof(del));

            if (Event is null)
                throw new InvalidOperationException($"This event is either not initialized or failed to initialize.");

            try { Event.RemoveEventHandler(del.Target, del); } catch { }

            try { Delegate = Event.DeclaringType.GetField(Event.Name).GetValue(null) as Delegate; } catch { }
        }

        public void Invoke(EventInfo eventInfo)
        {
            if (eventInfo is null)
                throw new ArgumentNullException(nameof(eventInfo));

            if (Invoker is null)
                throw new InvalidOperationException($"This event does not have a paired invoker.");

            _callBuffer[0] = eventInfo;

            Invoker.SafeCall(null, _callBuffer);

            _callBuffer[0] = null;
        }
    }
}