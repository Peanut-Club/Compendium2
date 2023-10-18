using System;

namespace Compendium.Plugins
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginAttribute : Attribute
    {
        public string Author { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Version Version { get; set; }

        public Version MinRequiredCoreVersion { get; set; }
        public Version MaxRequiredCoreVersion { get; set; }
    }
}