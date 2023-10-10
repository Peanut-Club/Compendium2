using System;
using System.Reflection;

namespace Compendium.Utilities.Reflection
{
    public static class EventUtilities
    {
        public static EventInfo[] GetAllEvents(this Type type)
            => type.GetEvents(MethodUtilities.BindingFlags);

        public static void VerifyEventType(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type == typeof(EventInfo))
                throw new ArgumentException($"Argument 'type' has to be a class inheriting from EventInfo, not the EventInfo class itself.");

            if (!type.InheritsType<EventInfo>())
                throw new ArgumentException($"Argument 'type' has to be a class inheriting from EventInfo.");
        }
    }
}