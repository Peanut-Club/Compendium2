using Compendium.Utilities.Reflection;

using System;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class EventInvokerAttribute : Attribute
    {
        public Type Event { get; }

        public EventInvokerAttribute(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            type.VerifyEventType();

            Event = type;
        }
    }
}