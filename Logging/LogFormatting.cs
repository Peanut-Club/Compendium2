using System;

namespace Compendium.Logging
{
    [Flags]
    public enum LogFormatting : byte
    {
        Source = 2,
        Message = 4
    }
}