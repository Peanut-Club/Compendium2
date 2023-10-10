using System;

namespace Compendium.Plugins.Configuration
{
    [Flags]
    public enum ConfigurationFlags : byte
    {
        None = 0,

        ConsiderMissingAsCorrupted = 2,
        ConsiderFailedAsCorrupted = 4,

        UseFileWatcher = 8
    }
}