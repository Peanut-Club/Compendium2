using Compendium.Components;
using Compendium.Plugins.Configuration;

using System.Reflection;

namespace Compendium.Plugins
{
    public class Plugin : Component
    {
        public virtual PluginOptions PluginOptions { get; } = PluginOptions.FromAssembly(Assembly.GetExecutingAssembly());
        public virtual ConfigurationOptions ConfigOptions { get; } = ConfigurationOptions.FromAssembly(Assembly.GetExecutingAssembly());

        public ConfigurationHandler Config { get; internal set; }

        public virtual void OnReload() { }
    }
}