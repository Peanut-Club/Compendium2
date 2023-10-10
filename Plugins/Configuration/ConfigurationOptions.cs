using System.Reflection;

namespace Compendium.Plugins.Configuration
{
    public class ConfigurationOptions
    {
        public bool Global { get; }
        public bool GlobalOnly { get; }

        public ConfigurationFlags Flags { get; }

        public string Name { get; }

        public ConfigurationOptions(bool global, bool globalOnly, string name, ConfigurationFlags flags = ConfigurationFlags.None)
        {
            Global = global;
            GlobalOnly = globalOnly;
            Name = name;
            Flags = flags;
        }

        public static ConfigurationOptions FromAssembly(Assembly assembly)
            => new ConfigurationOptions(true, false, assembly.GetName().Name);
    }
}