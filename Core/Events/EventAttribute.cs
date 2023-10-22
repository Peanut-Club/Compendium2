using System;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class EventAttribute : Attribute
    {
        public Type Type { get; internal set; }
        public EventData Event { get; internal set; }

        public EventAttribute() { }
        public EventAttribute(Type type)
            => Type = type;
    }
}