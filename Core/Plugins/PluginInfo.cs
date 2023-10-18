using Compendium.Assemblies;

using System;

namespace Compendium.Plugins
{
    public class PluginInfo
    {
        public string Author { get; }
        public string Name { get; }
        public string Description { get; }
        public string Hash { get; }
        public string Id { get; }

        public string FilePath { get; }
        public string ConfigPath { get; }
        public string ConfigId { get; }

        public object Instance { get; }

        public Version Version { get; }

        public Version MinRequiredCoreVersion { get; }
        public Version MaxRequiredCoreVersion { get; }

        public AssemblyInfo Assembly { get; }
        public Type Type { get; }

        public PluginInfo(
            string author,
            string name, 
            string description, 
            string hash, 
            string id, 
            string filePath, 
            string configPath, 
            string configId, 

            object instance,
            
            Version version, 
            Version minRequiredCoreVersion,
            Version maxRequiredCoreVersion,
            
            AssemblyInfo assembly,
            Type type)
        {
            Author = author;
            Name = name;
            Description = description;
            Hash = hash;
            Id = id;

            Instance = instance;

            FilePath = filePath;
            ConfigPath = configPath;
            ConfigId = configId;

            Version = version;

            MinRequiredCoreVersion = minRequiredCoreVersion;
            MaxRequiredCoreVersion = maxRequiredCoreVersion;

            Assembly = assembly;
            Type = type;
        }
    }
}