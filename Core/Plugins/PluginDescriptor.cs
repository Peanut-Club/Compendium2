using Compendium.Assemblies;
using Compendium.Configuration;
using Compendium.Events;
using Compendium.Patching;

using System;

namespace Compendium.Plugins
{
    public class PluginDescriptor
    {
        public AssemblyReference Assembly { get; }

        public PluginMetadata Metadata { get; }
        public PluginSymbols Symbols { get; }

        public Type Type { get; }

        public PatchDescriptor[] GetPatches() 
        { 

        }

        public EventListenerData[] GetEvents() 
        {

        }

        public ConfigurationTargetInfo[] GetConfigs() 
        { 

        }

        public PluginDescriptor(AssemblyReference assembly, PluginMetadata metadata, PluginSymbols symbols, Type type)
        {
            Assembly = assembly;
            Metadata = metadata;
            Symbols = symbols;
            Type = type;
        }
    }
}