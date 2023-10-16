using Compendium.Utilities.Reflection;

using System;

namespace Compendium.Events
{
    public class EventData
    {
        public Type Type { get; set; }   
        public Delegate Delegate { get; set; }
        public System.Reflection.EventInfo Event { get; set; }

        public EventData(Type eventType, System.Reflection.EventInfo eventInfo)
        {
            if (eventType is null)
                throw new ArgumentNullException(nameof(eventType));

            eventType.VerifyEventType();

            Type = eventType;
            Event = eventInfo;

            Delegate = Event.DeclaringType.GetField(Event.Name).GetValue(null) as Delegate;

            if (Event is null || Delegate is null)
                throw new ArgumentException($"Failed to find a handler corresponding to the event specified as 'eventType'.");
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
    }
}