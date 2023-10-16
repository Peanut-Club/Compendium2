using System.Reflection;

namespace Compendium.Configuration
{
    public class ConfigurationOptions
    {
        public ConfigurationFlags Flags { get; }

        public string Name { get; }

        public ConfigurationOptions(string name, ConfigurationFlags flags = ConfigurationFlags.None)
        {
            Name = name;
            Flags = flags;
        }

        public static ConfigurationOptions FromAssembly(Assembly assembly)
            => new ConfigurationOptions(assembly.GetName().Name);
    }
}