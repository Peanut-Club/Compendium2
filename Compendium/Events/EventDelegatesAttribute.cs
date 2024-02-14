using System;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EventDelegatesAttribute : Attribute
    {
        public Type Type { get; }

        public EventDelegatesAttribute(Type type)
            => Type = type;
    }
}