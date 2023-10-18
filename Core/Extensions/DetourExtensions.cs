using Compendium.Patching.Detours;

using System.Reflection;

namespace Compendium.Extensions
{
    public static class DetourExtensions
    {
        public static DetourHandler CreateDetour(this MethodBase target, MethodBase destination)
            => DetourHandler.Get(target, destination);

        public static DetourHandler CreateAndApplyDetour(this MethodBase target, MethodBase destination)
        {
            var handler = DetourHandler.Get(target, destination);

            if (!handler.IsApplied)
                handler.IsApplied = true;

            return handler;
        }

        public static void RemoveDetour(this MethodBase target, MethodBase destination)
            => DetourHandler.Get(target, destination)!.IsApplied = false;
    }
}