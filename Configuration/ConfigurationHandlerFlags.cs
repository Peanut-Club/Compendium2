using System;

namespace Compendium.Configuration
{
    [Flags]
    public enum ConfigurationHandlerFlags : byte
    {
        None = 0,

        ConsiderMissingAsCorrupted = 2,
        ConsiderFailedAsCorrupted = 4,

        UseFileWatcher = 8,

        CanBeGlobalFile = 16,
        CanBeGlobalFileOnly = 32,
    }
}