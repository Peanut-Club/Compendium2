using Compendium.Utilities.Reflection;

using System;

namespace Compendium.Events
{
    public static class EventHandlers
    {
        public static event Action<EventInfo> OnExample;

        public static void InvokeOnExample(EventInfo eventInfo)
            => OnExample.SafeCall(eventInfo);
    }
}