using Compendium.Utilities.Reflection;

using System;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
    public class EventDelegateAttribute : Attribute
    {
        public Type Type { get; }

        public EventDelegateAttribute(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            type.VerifyEventType();

            Type = type;
        }
    }
}