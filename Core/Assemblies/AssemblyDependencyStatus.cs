using System;

namespace Compendium.Assemblies
{
    [Flags]
    public enum AssemblyDependencyStatus : byte
    {
        Loaded = 0,
        Failed = 2,

        RemovedVersionCheck = 4,
        RemovedTokenCheck = 8,
        RemovedCultureCheck = 16
    }
}