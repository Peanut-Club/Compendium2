using System;

namespace Compendium.Patching
{
    [Flags]
    public enum PatchFlags : short
    {
        IsPrefix = 2,
        IsPostfix = 8,
        IsTranspiler = 16,
        IsIL = 32,
        IsFinalizer = 64,
        IsEvent = 128,

        Get = 256,
        Set = 512,
    }
}