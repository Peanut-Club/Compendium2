using System;
using System.Reflection;

namespace Compendium.Plugins
{
    public class PluginOptions
    {
        public string Name { get; }
        public string Author { get; }
        public string Description { get; }

        public Version Version { get; }

        public Version MinVersion { get; }
        public Version MaxVersion { get; }

        public bool RegisterCommands { get; }
        public bool RegisterEvents { get; }
        public bool RegisterPatches { get; }

        public bool GlobalLoad { get; }

        public PluginOptions(
            string name, 
            string author, 
            string description, 
            
            Version version, 
            Version minVersion, 
            Version maxVersion, 
            
            bool registerCommands, 
            bool registerEvents, 
            bool registerPatches,

            bool globalLoad)
        {
            Name = name;
            Author = author;
            Description = description;

            Version = version;
            MinVersion = minVersion;
            MaxVersion = maxVersion;

            RegisterCommands = registerCommands;
            RegisterEvents = registerEvents;
            RegisterPatches = registerPatches;

            GlobalLoad = globalLoad;
        }

        public static PluginOptions FromAssembly(Assembly assembly)
        {
            var name = assembly.GetName();

            return new PluginOptions(
                name.Name,

                assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? null,
                assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? null,

                name.Version,

                null,
                null,

                true,
                true,
                true,
                true);
        }
    }
}