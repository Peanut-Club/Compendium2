using System;

namespace Compendium.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ConfigurationAttribute : Attribute
    {
        public string Name { get; internal set; }

        public string Description { get; }

        public ConfigurationAttribute()
        {
            Name = null;
            Description = null;
        }

        public ConfigurationAttribute(string name, params string[] description)
        {
            Name = name;

            if (description.Length > 0)
                Description = string.Join("\n", description);
            else
                Description = "No description.";
        }
    }
}