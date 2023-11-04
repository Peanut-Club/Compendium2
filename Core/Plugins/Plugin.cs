using System;
using System.Reflection;

namespace Compendium.Plugins
{
    public class Plugin
    {
        public string Name { get; }
        public string Description { get; }
        public string Author { get; }
        public string Link { get; }

        public Version Version { get; }

        public Version MinVersion { get; }
        public Version MaxVersion { get; }

        public Type Type { get; }
        public Assembly Assembly { get; }

        public Plugin(
            string name, 
            string description, 
            string author, 
            string link, 
            
            Version version,
            Version minVersion,
            Version maxVersion, 
            
            Type type, 
            Assembly assembly)
        {
            Name = name;
            Description = description;
            Author = author;
            Link = link;

            Version = version;
            MinVersion = minVersion;
            MaxVersion = maxVersion;

            Type = type;
            Assembly = assembly;
        }
    }
}