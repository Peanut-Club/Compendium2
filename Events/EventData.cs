using Compendium.Utilities.Reflection;

using System;

namespace Compendium.Events
{
    public class EventData
    {
        private static System.Reflection.EventInfo[] _handlersEventsValue;

        public static System.Reflection.EventInfo[] Events { get => _handlersEventsValue ??= typeof(Handlers).GetAllEvents(); }

        public virtual Type Type { get; set; }   
        public virtual System.Reflection.EventInfo Event { get; set; }

        public virtual CallStats<EventData> Stats { get; private set; }

        public EventData(Type eventType)
            => Initialize(eventType);

        public virtual void Initialize(Type eventType)
        {
            if (eventType is null)
                throw new ArgumentNullException(nameof(eventType));

            eventType.VerifyEventType();

            Type = eventType;

            for (int i = 0; i < Events.Length; i++)
            {
                var evType = Events[i].EventHandlerType.GetGenericType();

                if (evType is null || evType != eventType)
                    continue;

                Event = Events[i];
            }

            if (Event is null)
                throw new ArgumentException($"Failed to find a handler corresponding to the event specified as 'eventType'.");

            Stats = CallStats<EventData>.Create(this);
        }

        public virtual void AddHandler(Delegate del)
        {
            if (del is null)
                throw new ArgumentNullException(nameof(del));

            if (Event is null)
                throw new InvalidOperationException($"This event is either not initialized or failed to initialize.");

            try { Event.RemoveEventHandler(del.Target, del); } catch { }
            try { Event.AddEventHandler(del.Target, del); } catch { }
        }

        public virtual void RemoveHandler(Delegate del)
        {
            if (del is null)
                throw new ArgumentNullException(nameof(del));

            if (Event is null)
                throw new InvalidOperationException($"This event is either not initialized or failed to initialize.");

            try { Event.RemoveEventHandler(del.Target, del); } catch { }
        }
    }
}