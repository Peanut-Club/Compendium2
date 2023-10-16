using Compendium.Utilities.Reflection;

using MonoMod.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compendium.Events
{
    public static class EventManager
    {
        private static readonly HashSet<EventData> _events = new HashSet<EventData>();

        public static IReadOnlyCollection<EventData> Events => _events;

        static EventManager()
        {
            foreach (var evInfo in typeof(EventHandlers).GetAllEvents())
            {
                var genericType = evInfo.EventHandlerType.GetFirstGenericType();

                if (genericType is null || !genericType.InheritsType<EventInfo>())
                    continue;

                _events.Add(new EventData(genericType, evInfo));
            }
        }

        public static bool RegisterEvent(EventData data)
        {
            if (_events.Any(ev => ev == data))
                return false;

            return _events.Add(data);
        }

        public static bool UnregisterEvent(EventData data)
            => _events.RemoveWhere(ev => ev == data) > 0;

        public static EventData GetEvent(Type infoType)
        {
            foreach (var ev in _events)
            {
                if (ev.Type == infoType)
                    return ev;
            }

            return null;
        }

        public static void RegisterHandler(MethodBase target, object handle, EventData eventData)
        {
            var methodParams = target.GetParameters();
            var del = target.CreateDelegate(typeof(Action<>).MakeGenericType(methodParams[0].ParameterType), handle);

            eventData.AddHandler(del);
        }

        public static void UnregisterHandler(MethodBase target, object handle)
        {
            foreach (var ev in _events)
            {
                var list = ev.Delegate.GetInvocationList();

                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].Method == target && ObjectUtilities.IsInstance(handle, list[i].Target))
                    {
                        ev.RemoveHandler(list[i]);
                    }
                }
            }
        }
    }
}