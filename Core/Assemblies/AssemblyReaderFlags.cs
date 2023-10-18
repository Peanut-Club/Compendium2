using System;

namespace Compendium.Assemblies
{
    [Flags]
    public enum AssemblyReaderFlags : byte
    {
        None = 0,

        IgnoreCulture = 2,
        IgnoreToken = 4,
        IgnoreVersion = 8
    }
}