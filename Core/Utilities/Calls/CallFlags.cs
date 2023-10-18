using System;

namespace Compendium.Utilities.Calls
{
    [Flags]
    public enum CallFlags : byte
    {
        None = 0,
        EnableProfiler = 2,
        EnableDelay = 4
    }
}