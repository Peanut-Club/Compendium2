using Compendium.Attributes;

using System;

namespace Compendium.Plugins.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class ConfigurationAttribute : ResolveableAttribute<ConfigurationAttribute>
    {
    }
}