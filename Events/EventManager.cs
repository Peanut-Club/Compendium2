using Compendium.Utilities.Reflection;

using System;
using System.Collections.Generic;

namespace Compendium.Events
{
    public static class EventManager
    {
        private static readonly HashSet<EventData> _events = new HashSet<EventData>();

        public static IReadOnlyCollection<EventData> Events => _events;

        public static bool IsAny(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            type.VerifyEventType();


        }
    }
}