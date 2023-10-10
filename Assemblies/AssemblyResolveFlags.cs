using System;

namespace Compendium.Assemblies
{
    [Flags]
    public enum AssemblyResolveFlags
    {
        None = 0,

        RemoveDependencyVersionCheck = 2,
        RemoveDependencyTokenCheck = 4,
        RemoveDependencyCultureCheck = 8
    }
}