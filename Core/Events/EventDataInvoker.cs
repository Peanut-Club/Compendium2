using Compendium.Utilities.Reflection;

using System;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class EventDataInvokerAttribute : Attribute
    {
        public Type Event { get; }

        public EventDataInvokerAttribute(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            type.VerifyEventType();

            Event = type;
        }
    }
}