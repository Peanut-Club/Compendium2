using System;
using System.Reflection;

namespace Compendium.Plugins
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public string Author { get; }

        public Version Version { get; }

        public Version Minimal { get; }
        public Version Maximal { get; }

        public Assembly Assembly { get; }
        public AssemblyName AssemblyName { get; }

        public PluginAttribute(
            string name = null, 

            string description = null, 
            string author = null, 
            
            Version version = null, 
            Version minimal = null,
            Version maximal = null)
        {
            Assembly = Assembly.GetExecutingAssembly();
            AssemblyName = Assembly.GetName();

            Name = string.IsNullOrWhiteSpace(name) ? AssemblyName.Name : name;

            Description = string.IsNullOrWhiteSpace(description) ? Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description : description;
            Author = string.IsNullOrWhiteSpace(author) ? Assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company : author;

            Version = version ?? AssemblyName.Version;

            Minimal = minimal ?? PluginManager.AssemblyName.Version;
            Maximal = maximal ?? PluginManager.AssemblyName.Version;
        }
    }
}