using System;

namespace Compendium.Attributes
{
    [Flags]
    public enum AttributeResolveFlags : byte
    {
        Type = 0,
        Method = 2,
        Field = 4,
        Property = 8
    }
}