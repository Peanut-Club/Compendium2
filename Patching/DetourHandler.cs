using Compendium.Utilities;
using MonoMod.Core;

using System.Collections.Generic;
using System.Reflection;

namespace Compendium.Patching
{
    public class DetourHandler : Disposable
    {
        private static readonly List<DetourHandler> _handlers = new List<DetourHandler>();

        public MethodBase Target { get; }
        public MethodBase Destination { get; }

        public bool IsApplied
        {
            get => Detour != null && Detour.IsApplied;
            set
            {
                if (!value && IsApplied)
                {
                    Detour.Undo();
                    Detour.Dispose();
                    Detour = null;
                }
                else if (value && !IsApplied)
                {
                    Detour = DetourFactory.Current.CreateDetour(Request);
                    Detour.Apply();
                }
            }
        }

        internal CreateDetourRequest Request { get; }
        internal ICoreDetour Detour { get; set; }

        public DetourHandler(MethodBase target, MethodBase destination)
        {
            Target = target;
            Destination = destination;

            Request = new CreateDetourRequest(target, destination) { ApplyByDefault = false };

            _handlers.Add(this);
        }

        public override void DisposeInternal()
        {
            IsApplied = false;

            if (Detour != null)
            {
                Detour.Dispose();
                Detour = null;
            }

            _handlers.Remove(this);

            base.DisposeInternal();
        }

        public static DetourHandler Get(MethodBase target, MethodBase destination)
        {
            for (int i = 0; i < _handlers.Count; i++)
            {
                if (_handlers[i].Target == target 
                    && _handlers[i].Destination == destination)
                    return _handlers[i];
            }

            return new DetourHandler(target, destination);
        }
    }
}
