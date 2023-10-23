using Compendium.Logging;

using System;
using System.Reflection;

namespace Compendium.Utilities.Reflection
{
    public static class EventUtilities
    {
        public static EventInfo Event(this Type type, string eventName)
            => type.GetEvent(eventName, MethodUtilities.BindingFlags);

        public static EventInfo[] GetAllEvents(this Type type)
            => type.GetEvents(MethodUtilities.BindingFlags);

        public static void AddHandler(this Type type, string eventName, MethodBase method, object target)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentNullException(nameof(eventName));

            var ev = type.Event(eventName);

            if (ev is null)
                throw new ArgumentException($"Failed to find an event of name '{eventName}' in class '{type.ToName()}'");

            if (!method.TryCreateDelegate(target, ev.EventHandlerType, out var del))
                return;

            try
            {
                ev.RemoveEventHandler(target, del);
            }
            catch (Exception ex)
            {
                Log.Error("Event Utilities", $"Failed to remove event handler '{method.ToName()}' from event '{ev.ToName()}'", ex);
            }

            try
            {
                ev.AddEventHandler(target, del);
            }
            catch (Exception ex)
            {
                Log.Error("Event Utilities", $"Failed to add event handler '{method.ToName()}' to event '{ev.ToName()}'", ex);
            }
        }

        public static void AddHandler(this Type type, string eventName, MethodBase method)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentNullException(nameof(eventName));

            var ev = type.Event(eventName);

            if (ev is null)
                throw new ArgumentException($"Failed to find an event of name '{eventName}' in class '{type.ToName()}'");

            if (!method.TryCreateDelegate(ev.EventHandlerType, out var del))
                return;

            try
            {
                ev.RemoveEventHandler(null, del);
            }
            catch (Exception ex)
            {
                Log.Error("Event Utilities", $"Failed to remove event handler '{method.ToName()}' from event '{ev.ToName()}'", ex);
            }

            try
            {
                ev.AddEventHandler(null, del);
            }
            catch (Exception ex)
            {
                Log.Error("Event Utilities", $"Failed to add event handler '{method.ToName()}' to event '{ev.ToName()}'", ex);
            }
        }

        public static void AddHandler(this Type type, string eventName, Delegate handler)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            var ev = type.Event(eventName);

            if (ev is null)
                throw new ArgumentException($"Failed to find an event of name '{eventName}' in class '{type.ToName()}'");

            try
            {
                ev.RemoveEventHandler(handler.Target, handler);
            }
            catch (Exception ex)
            {
                Log.Error("Event Utilities", $"Failed to remove event handler '{handler.GetMethodInfo().ToName()}' from event '{ev.ToName()}'", ex);
            }

            try
            {
                ev.AddEventHandler(handler.Target, handler);
            }
            catch (Exception ex)
            {
                Log.Error("Event Utilities", $"Failed to add event handler '{handler.GetMethodInfo().ToName()}' to event '{ev.ToName()}'", ex);
            }
        }

        public static void RemoveHandler(this Type type, string eventName, Delegate handler)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            var ev = type.Event(eventName);

            if (ev is null)
                throw new ArgumentException($"Failed to find an event of name '{eventName}' in class '{type.ToName()}'");

            try
            {
                ev.RemoveEventHandler(handler.Target, handler);
            }
            catch (Exception ex)
            {
                Log.Error("Event Utilities", $"Failed to remove event handler '{handler.GetMethodInfo().ToName()}' from event '{ev.ToName()}'", ex);
            }
        }

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