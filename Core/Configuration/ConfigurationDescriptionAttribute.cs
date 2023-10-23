using System;

namespace Compendium.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ConfigurationDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public ConfigurationDescriptionAttribute(params string[] descriptionLines)
        {
            if (descriptionLines.Length <= 0)
                Description = null;
            else if (descriptionLines.Length == 1)
                Description = descriptionLines[0];
            else
                Description = string.Join("\n", descriptionLines);
        }
    }
}