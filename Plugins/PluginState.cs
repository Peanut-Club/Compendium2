using System;

namespace Compendium.Plugins
{
    [Flags]
    public enum PluginState
    {
        Loaded = 0,
        Unloaded = 2,
        Enabled = 4,
        Disabled = 8
    }
}