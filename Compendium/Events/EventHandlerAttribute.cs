using System;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EventHandlerAttribute : Attribute
    {
        public Type Event { get; }

        public EventHandlerAttribute(Type eventType = null)
            => Event = eventType;
    }
}