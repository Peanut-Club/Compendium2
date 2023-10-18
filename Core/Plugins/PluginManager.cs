using Compendium.Assemblies;
using Compendium.Logging;
using Compendium.Utilities;
using Compendium.Utilities.Instances;
using Compendium.Utilities.Reflection;

using MonoMod.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Compendium.Plugins
{
    public static class PluginManager
    {
        private static readonly HashSet<PluginInfo> _plugins = new HashSet<PluginInfo>();

        public static void LoadFrom(string directory)
        {
            var assemblies = AssemblyManager.ReadDirectory(directory, AssemblyReaderFlags.IgnoreCulture | AssemblyReaderFlags.IgnoreVersion | AssemblyReaderFlags.IgnoreToken);

            if (assemblies.Length > 0)
            {
                for (int i = 0; i < assemblies.Length; i++)
                {
                    if (assemblies[i].Status != AssemblyStatus.Loaded || assemblies[i].Assembly is null)
                    {
                        Log.Error("Plugin Manager", $"Cannot load plugin '{assemblies[i].Definition.Name.Name}' due to: {assemblies[i].Status.ToString().SpaceByUpperCase()}");

                        for (int x = 0; x < assemblies[i].Dependencies.Length; x++)
                            Log.Error("Plugin Manager", $"Plugin '{assemblies[i].Definition.Name.Name}' dependency [{x}]: '{assemblies[i].Dependencies[x].Reference.FullName}' ({assemblies[i].Dependencies[x].Status})");
                    }
                    else
                    {
                        LoadFromAssembly(assemblies[i]);
                    }
                }
            }
        }

        public static void LoadFromAssembly(AssemblyInfo assembly)
        {
            foreach (var type in assembly.Assembly.GetTypes())
            {
                if (!type.HasAttribute<PluginAttribute>(out var pluginAttribute))
                    continue;

                object instance = null;

                if (!type.IsStatic())
                    instance = InstanceTracker.Get(type, null, true);

                if (!type.IsStatic() && instance is null)
                {
                    Log.Error("Plugin Manager", $"Cannot load plugin type '{type.FullName}' due to: Failed to create class instance");
                    continue;
                }

                LoadFromType(type, instance, pluginAttribute, assembly.Path);
            }
        }

        public static void LoadFromType(Type type, object instance, PluginAttribute pluginAttribute, string path)
        {
            if (_plugins.Any(pl => pl.Type == type))
            {
                Log.Error("Plugin Manager", $"Cannot load plugin type '{type.FullName}' due to: There already is a plugin from class '{type.ToName()}'");
                return;
            }

            if (_plugins.Any(pl => pl.Name == pluginAttribute.Name))
            {
                Log.Error("Plugin Manager", $"Cannot load plugin type '{type.FullName}' due to: There already is a plugin with the same name.");
                return;
            }

            var plugin = new PluginInfo(
                pluginAttribute.Author ?? "No author specified.",
                pluginAttribute.Name ?? type.Assembly.GetName().Name,
                pluginAttribute.Description ?? "No description specified.",

                type.Assembly.GetRuntimeHashedFullName(),

                RandomGeneration.String(10),

                path,

                null,

                RandomGeneration.String(10),

                instance,

                pluginAttribute.Version ?? type.Assembly.GetName().Version,

                pluginAttribute.MinRequiredCoreVersion,
                pluginAttribute.MaxRequiredCoreVersion,

                AssemblyManager.ReadAssembly(type.Assembly),

                type);
        }
    }
}