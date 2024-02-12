using Common.Extensions;
using Common.IO.Collections;

using System;
using System.Reflection;

namespace Compendium.Events
{
    internal static class EventUtils
    {
        internal static EventInfo GetEvent(MethodInfo method, LockedDictionary<Type, EventInfo> events)
        {
            Type eventType = null;

            if (method.HasAttribute<EventHandlerAttribute>(out var eventHandlerAttribute)
                && eventHandlerAttribute.Event != null)
                eventType = eventHandlerAttribute.Event;
            else
            {
                var parameters = method.Parameters();

                if (parameters.Length == 1 && parameters[0].ParameterType.IsSubclassOf(typeof(Event)))
                    eventType = parameters[0].ParameterType;
            }

            if (eventType != null && events.TryGetValue(eventType, out var eventInfo))
                return eventInfo;

            return null;
        }
    }
}
