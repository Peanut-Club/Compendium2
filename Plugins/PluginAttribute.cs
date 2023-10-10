using Compendium.Attributes;
using Compendium.Plugins.Configuration;

using System;
using System.Reflection;

namespace Compendium.Plugins
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginAttribute : ResolveableAttribute<PluginAttribute>
    {
        public PluginOptions PluginOptions { get; }
        public ConfigurationOptions ConfigOptions { get; }

        public PluginAttribute(PluginOptions options, ConfigurationOptions configurationOptions)
        {         
            PluginOptions = options ?? PluginOptions.FromAssembly(Assembly.GetExecutingAssembly());
            ConfigOptions = configurationOptions ?? ConfigurationOptions.FromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}