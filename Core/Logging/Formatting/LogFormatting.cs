using System;

namespace Compendium.Logging.Formatting
{
    [Flags]
    public enum LogFormatting : byte
    {
        Source = 2,
        Message = 4
    }
}