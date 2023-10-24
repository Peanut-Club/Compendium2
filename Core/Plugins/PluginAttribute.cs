using System;
using System.Reflection;

namespace Compendium.Plugins
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginAttribute : Attribute
    {
        public PluginMetadata Metadata { get; }

        public PluginAttribute()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetName();

            Metadata = new PluginMetadata
            {
                Name = name.Name,

                Author = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Unknown",
                Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "Unknown",

                Version = new PluginVersion
                {
                    Major = name.Version?.Major ?? 1,
                    Minor = name.Version?.Minor ?? 0,
                    Build = name.Version?.Build ?? 0,
                    Patch = name.Version?.Revision ?? 0
                },

                MaxVersion = default,
                MinVersion = default
            };
        }

        public PluginAttribute(PluginMetadata metadata)
        {
            Metadata = metadata;
        }

        public PluginAttribute(string name, string author, string description, PluginVersion version, PluginVersion maxVersion = default, PluginVersion minVersion = default)
            : this(new PluginMetadata
            {
                Name = name,
                Author = author,
                Description = description,
                Version = version,
                MaxVersion = maxVersion,
                MinVersion = minVersion
            })
        { }
    }
}