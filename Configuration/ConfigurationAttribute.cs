using Compendium.Attributes;
using Compendium.Utilities;

using System;

namespace Compendium.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class ConfigurationAttribute : ResolveableAttribute<ConfigurationAttribute>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public string Id { get; set; }

        public ConfigurationFlags Flags { get; private set; }

        public ConfigurationAttribute(ConfigurationFlags flags = ConfigurationFlags.None)
        {
            Name = null;
            Description = "No description.";
            Flags = flags;
        }

        public ConfigurationAttribute(string name, params string[] description)
        {
            Name = name;
            Description = description != null && description.Length > 0 ? description.Compile() : "No description.";
            Flags = ConfigurationFlags.None;
        }

        public ConfigurationAttribute(string name, ConfigurationFlags flags, params string[] description)
        {
            Name = name;
            Description = description != null && description.Length > 0 ? description.Compile() : "No description."; ;
            Flags = flags;
        }

        public override void OnResolved(AttributeInfo<ConfigurationAttribute> attributeInfo)
        {
            base.OnResolved(attributeInfo);

            if (string.IsNullOrWhiteSpace(Name))
            {
                var name = "";

                if (Flags == ConfigurationFlags.None)
                    name = attributeInfo.Field?.Name ?? attributeInfo.Property?.Name;
                else
                {
                    if (Flags.Any(ConfigurationFlags.Name_SnakeCase))
                        name = (attributeInfo.Field?.Name ?? attributeInfo.Property?.Name).SnakeCase();
                    else if (Flags.Any(ConfigurationFlags.Name_Lowered))
                        name = (attributeInfo.Field?.Name ?? attributeInfo.Property?.Name).ToLowerInvariant();
                    else
                        name = attributeInfo.Field?.Name ?? attributeInfo.Property?.Name;
                }

                Name = name;
            }

            Description ??= "No description.";

            var type = attributeInfo.Property?.PropertyType ?? attributeInfo.Field?.FieldType;

            if (Flags.Any(ConfigurationFlags.Description_IncludeValueType))
                Description += $"\nType: {type.Name}";

            if (Flags.Any(ConfigurationFlags.Description_IncludeEnumValues) && type.IsEnum)
            {
                Description += $"\nPossible values: ";

                var values = Enum.GetValues(type);

                for (int i = 0; i < values.Length; i++)
                    Description += $"'{values.GetValue(i)}', ";

                Description = Description.TrimEnd(' ', ',');
            }
        }
    }
}