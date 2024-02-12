using Common.Extensions;
using Common.Logging;
using Common.Patching;
using Common.Utilities;
using Common.IO.Collections;

using System;

namespace Compendium.Events
{
    public class EventInfo
    {
        public static LogOutput Log => EventManager.Log;

        public LockedList<EventMethod> Listeners { get; } = new LockedList<EventMethod>();

        public Type Type { get; }
        public PatchInfo Patch { get; }
        
        public System.Reflection.EventInfo Event { get; }

        public string Name => Type.Name;

        public bool HasAnyCustom { get; set; }

        public bool HasAnyListeners
        {
            get => Listeners.Count > 0;
        }

        public bool HasPatch
        {
            get => Patch != null;
        }

        public bool IsPatched
        {
            get => Patch?.IsActive ?? false;
            set => CodeUtils.InlinedElse(value, Patch is null || value == IsPatched, () => PatchManager.Patch(Patch), () => PatchManager.Unpatch(Patch), () => Log.Info($"Patched method '{Patch.Target.ToName()}' (event: {Name})"), () => Log.Info($"Unpatched method '{Patch.Target.ToName()}' (event: {Name})"));
        }

        public EventInfo(Type type, PatchInfo patch, System.Reflection.EventInfo ev)
        {
            Type = type;
            Event = ev;
            Patch = patch;
        }

        public void Invoke(Event ev)
        {
            if (ev is null)
                return;

            for (int i = 0; i < Listeners.Count; i++)
            {
                var previousListener = i == 0 ? null : Listeners[i - 1];
                var currentListener = Listeners[i];
                var nextListener = !IndexUtils.IsValidIndex(i + 1, Listeners.Count) ? null : Listeners[i + 1];

                ev.UpdateHandlers(previousListener, currentListener, nextListener);

                currentListener.Invoke(ev);
            }

            if (Event != null)
                Event.Raise(null, ev);
        }
    }
}