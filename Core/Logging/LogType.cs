using System;

namespace Compendium.Logging
{
    [Flags]
    public enum LogType : short
    {
        Raw = 2,

        Trace = 4,

        Verbose = 8,

        Debug = 16,

        Information = 32,

        Warning = 64,

        Error = 128,

        Critical = 256
    }
}