using System;

namespace Compendium.Commands.Arguments
{
    [Flags]
    public enum CommandArgumentFlags : byte
    {
        None = 0,

        IsOptional = 2
    }
}