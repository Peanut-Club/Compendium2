using Compendium.Extensions;
using HarmonyLib;

using MonoMod.Utils;

using System;
using System.Linq;
using System.Reflection;

namespace Compendium.Patching
{
    public class PatchInfo
    {
        public Harmony Harmony { get; }

        public string Name { get; }

        public PatchFlags Flags { get; }

        public MethodInfo Target { get; }
        public MethodInfo Method { get; }
        public MethodInfo Patch { get; private set; }

        public PatchInfo(
            Harmony harmony, 
            
            string name, 
            
            PatchFlags flags, 
            
            MethodInfo target, 
            MethodInfo method)
        {
            Harmony = harmony;
            Name = name;
            Flags = flags;
            Target = target;
            Method = method;
        }

        public bool IsApplied
        {
            get
            {
                var info = Harmony.GetPatchInfo(Target);

                if (Flags.Has(PatchFlags.IsTranspiler) && info.Transpilers.Any(t => t.PatchMethod != null && t.PatchMethod == Method))
                    return true;

                if (Flags.Has(PatchFlags.IsPrefix) && info.Prefixes.Any(p => p.PatchMethod != null && p.PatchMethod == Method))
                    return true;

                if (Flags.Has(PatchFlags.IsPostfix) && info.Prefixes.Any(p => p.PatchMethod != null && p.PatchMethod == Method))
                    return true;

                if (Flags.Has(PatchFlags.IsIL) && info.ILManipulators.Any(il => il.PatchMethod != null && il.PatchMethod == Method))
                    return true;

                if (Flags.Has(PatchFlags.IsFinalizer) && info.Finalizers.Any(f => f.PatchMethod != null && f.PatchMethod == Method))
                    return true;

                return false;
            }
        }

        public void Apply()
        {
            if (Patch != null)
                return;

            if (Flags.Has(PatchFlags.IsTranspiler))
                Patch = Harmony.Patch(Target, null, null, new HarmonyMethod(Method));
            else if (Flags.Has(PatchFlags.IsIL))
                Patch = Harmony.Patch(Target, null, null, null, null, new HarmonyMethod(Method));
            else if (Flags.Has(PatchFlags.IsFinalizer))
                Patch = Harmony.Patch(Target, null, null, new HarmonyMethod(Method));
            else if (Flags.Has(PatchFlags.IsPrefix))
                Patch = Harmony.Patch(Target, new HarmonyMethod(Method));
            else if (Flags.Has(PatchFlags.IsPostfix))
                Patch = Harmony.Patch(Target, null, new HarmonyMethod(Method));
            else
                throw new InvalidOperationException($"Patch '{Name}' does not have a valid type flag.");

            Patcher.Logger.Debug($"Applied patch '{Name}'");
        }

        public void Unapply()
        {
            if (Patch is null)
                return;

            Harmony.Unpatch(Target, Patch);

            Patcher.Logger.Debug($"Removed patch '{Name}'");
        }
    }
}