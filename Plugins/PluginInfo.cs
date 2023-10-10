using Compendium.Assemblies;
using Compendium.Attributes;
using Compendium.Plugins.Configuration;

using System;

namespace Compendium.Plugins
{
    public class PluginInfo
    {
        public AssemblyInfo Assembly { get; }

        public Type Type { get; }

        public PluginOptions PluginOptions { get; }
        public ConfigurationOptions ConfigOptions { get; }

        public ConfigurationHandler Config { get; }

        public object Instance { get; }

        public PluginState State { get; internal set; } = PluginState.Unloaded;

        public PluginInfo(AssemblyInfo assembly, Type type, PluginOptions pluginOptions, ConfigurationOptions configOptions, ConfigurationHandler config, object instance = null)
        {
            Assembly = assembly;

            Type = type;

            PluginOptions = pluginOptions;
            ConfigOptions = configOptions;

            Config = config;

            Instance = instance;
        }

        public static PluginInfo FromAttribute(AssemblyInfo assembly, AttributeInfo<PluginAttribute> attribute, ConfigurationHandler config, object instance = null)
            => new PluginInfo(assembly, attribute.Type, attribute.Attribute.PluginOptions, attribute.Attribute.ConfigOptions, config, instance);

        public static PluginInfo FromClass(AssemblyInfo assembly, Plugin plugin, ConfigurationHandler config)
            => new PluginInfo(assembly, plugin.GetType(), plugin.PluginOptions, plugin.ConfigOptions, config, plugin);
    }
}
