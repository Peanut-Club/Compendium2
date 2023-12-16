using System;
using System.Reflection;

namespace Compendium.Plugins
{
    public class Plugin
    {
        public string Author { get; } = "default";
        public string Description { get; } = "default";
        public string Name { get; } = "default";

        public object Instance { get; }

        public Version Version { get; } = new Version(1, 0, 0, 0);

        public Version Maximal { get; } = new Version(1, 0, 0, 0);
        public Version Minimal { get; } = new Version(1, 0, 0, 0);

        public Assembly Assembly { get; }
        public Type Type { get; }

        public Action Load { get; }
        public Action Unload { get; }
        public Action Reload { get; }

        public Plugin(
            string author, 
            string description, 
            string name, 

            object instance,
            
            Version version, 
            Version maximal, 
            Version minimal, 
            
            Assembly assembly, 
            Type type,
            
            Action load,
            Action unload,
            Action reload)
        {
            Author = author;
            Description = description;
            Name = name;

            Instance = instance;

            Version = version;
            Maximal = maximal;
            Minimal = minimal;

            Assembly = assembly;
            Type = type;

            Load = load;
            Unload = unload;
            Reload = reload;
        }
    }
}