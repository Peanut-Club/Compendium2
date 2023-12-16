using Common.Logging;
using Common.Extensions;
using Common.Instances;
using Common.Pooling.Pools;

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;

namespace Compendium.Plugins
{
    public static class PluginManager
    {
        private static readonly List<Plugin> plugins = new List<Plugin>();
        private static readonly Queue<Assembly> reflection = new Queue<Assembly>();

        public static Assembly Assembly { get; }
        public static AssemblyName AssemblyName { get; }
        public static Version Version { get; }

        public static LogOutput Log { get; } = new LogOutput("Plugin Manager");

        public static IReadOnlyList<Plugin> Plugins => plugins;

        public static event Action<Assembly, Plugin> OnLoaded;
        public static event Action<Assembly, Plugin> OnUnloaded;

        public static void Unload(Plugin plugin)
        {
            plugin.Unload.Call(null, ex => Log.Error($"Failed to invoke 'Unload' method of plugin '{plugin.Name}':\n{ex}"));
            plugins.Remove(plugin);
        }

        public static void Load(string directoryPath)
        {
            DependencyManager.Load($"{directoryPath}/dependencies");

            Log.Info($"Loading plugins from '{directoryPath}' ..");

            foreach (var file in Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories))
                LoadFile(file);

            Log.Info($"Finished loading plugins! ({plugins.Count} plugins)");

            if (reflection.Count > 0)
            {
                Log.Info($"Plugin loading finished, but there are still {reflection.Count} plugins with missing dependencies. Attempting to reload now.");

                while (reflection.TryDequeue(out var assembly))
                    DoReflectionOnly(assembly, true);

                Log.Info($"Finished loading plugins with missing dependencies.");
            }
        }

        public static void LoadFile(string filePath)
        {
            if (Path.GetDirectoryName(filePath).Contains("dependencies"))
                return;

            Log.Trace($"Loading assembly file: {Path.GetFileName(filePath)}");

            try
            {
                var bytes = File.ReadAllBytes(filePath);

                if (bytes is null || bytes.Length <= 0)
                    return;

                var assembly = Assembly.ReflectionOnlyLoad(bytes);

                if (assembly is null)
                    return;

                LoadAssembly(assembly, true);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to load assembly from file '{Path.GetFileName(filePath)}':\n{ex}");
            }
        }

        public static void LoadAssembly(Assembly assembly, bool isReflection = false)
        {
            if (isReflection && assembly.ReflectionOnly)
            {
                DoReflectionOnly(assembly);
                return;
            }
            else if (isReflection && !assembly.ReflectionOnly)
            {
                Log.Warn($"Attempted to perform a reflection-only load on an assembly loaded without the reflection-only context! (assembly: {assembly.FullName})");
                return;
            }
            else if (!isReflection && assembly.ReflectionOnly)
            {
                Log.Warn($"Attempted to perform a normal laod on an assembly loaded via the reflection-only context! (assembly: {assembly.FullName})");
                return;
            }
            else
            {
                var foundPlugins = ListPool<Plugin>.Shared.Next();
                var assemblyName = assembly.GetName();

                foreach (var type in assembly.GetTypes())
                {
                    Log.Trace($"Loading type: {type.FullName}");

                    if (!type.HasAttribute<PluginAttribute>(out var pluginAttribute))
                        continue;

                    if (pluginAttribute.Minimal > Version)
                    {
                        Log.Warn($"Cannot load plugin '{pluginAttribute.Name}', minimal required Compendium version by plugin: {pluginAttribute.Minimal} (current: {Version})");
                        continue;
                    }

                    if (pluginAttribute.Maximal < Version)
                    {
                        Log.Warn($"Cannot load plugin '{pluginAttribute.Name}', maximal required Compendium version by plugin: {pluginAttribute.Maximal} (current: {Version})");
                        continue;
                    }                    

                    if (!ValidatePlugin(type, out var instance, out var reload, out var unload, out var load))
                        continue;

                    var plugin = new Plugin(
                        pluginAttribute.Author,
                        pluginAttribute.Description,
                        pluginAttribute.Name,

                        instance,

                        pluginAttribute.Version,
                        pluginAttribute.Maximal,
                        pluginAttribute.Minimal,

                        assembly,
                        type,

                        load,
                        unload,
                        reload);

                    var shouldContinue = true;

                    for (int i = 0; i < plugins.Count; i++)
                    {
                        if (plugins[i].Name != plugin.Name)
                            continue;

                        if (plugins[i].Version > plugin.Version)
                        {
                            shouldContinue = false;
                            Log.Warn($"Attempted to load an outdated plugin version! There's already a newer version of '{plugins[i].Name}' present: {plugins[i].Version} (attempted to load '{plugin.Version})'");
                            continue;
                        }

                        if (plugins[i].Version < plugin.Version)
                        {
                            Log.Info($"Loading a newer version of plugin '{plugin.Name}' ({plugin.Version}), unloading the current version ({plugins[i].Version}).");
                            Unload(plugins[i]);
                            continue;
                        }
                    }

                    if (!shouldContinue)
                        continue;

                    foundPlugins.Add(plugin);

                    Log.Info($"Found plugin '{plugin.Name}' by '{plugin.Author}' ({plugin.Version}) in {assemblyName.Name}");
                }

                if (foundPlugins.Count <= 0)
                {
                    Log.Warn($"No valid plugin classes were found in assembly '{assemblyName.Name}'!");
                    return;
                }

                foreach (var plugin in foundPlugins)
                {
                    plugin.Load.Call(() =>
                    {
                        plugins.Add(plugin);

                        OnLoaded.Call(assembly, plugin);

                        Log.Info($"Loaded plugin '{plugin.Name}' by '{plugin.Author}' ({plugin.Version})!");
                    },
                    ex =>
                    {
                        Log.Error($"Failed to invoke 'Load' method of plugin '{plugin.Name}', skipping load.\n{ex}");
                    });
                }

                ListPool<Plugin>.Shared.Return(foundPlugins);
            }
        }

        private static void DoReflectionOnly(Assembly assembly, bool isReload = false)
        {
            DependencyManager.LoadAssembly(assembly, true);

            var dependencies = assembly.GetReferencedAssemblies();

            if (dependencies is null || dependencies.Length <= 0)
            {
                DoLoad(assembly.GetName());
                return;
            }

            var missing = ListPool<AssemblyName>.Shared.Next();

            for (int i = 0; i < dependencies.Length; i++)
            {
                ModifyName(dependencies[i]);

                if (!DependencyManager.TryGet(dependencies[i], out _))
                {
                    missing.Add(dependencies[i]);
                    continue;
                }
            }

            if (missing.Count > 0)
            {
                if (!isReload)
                {
                    reflection.Enqueue(assembly);
                    Log.Warn($"Failed to load plugin '{assembly.GetName().Name}' due to missing dependencies ({missing.Count}), retrying after all plugins get loaded:");
                }
                else
                    Log.Warn($"Failed to load plugin '{assembly.GetName().Name}' due to missing dependencies ({missing.Count}):");

                for (int i = 0; i < missing.Count; i++)
                    Log.Warn(missing[i].FullName);

                ListPool<AssemblyName>.Shared.Return(missing);
                return;
            }

            ListPool<AssemblyName>.Shared.Return(missing);

            DoLoad(assembly.GetName());
        }

        private static void DoLoad(AssemblyName assemblyName, bool isModified = false)
        {
            try
            {
                if (!isModified)
                    ModifyName(assemblyName);

                var assembly = Assembly.Load(assemblyName);

                if (assembly is null)
                    return;

                LoadAssembly(assembly, false);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to load assembly '{assemblyName.Name}':\n{ex}");
            }
        }

        private static void ModifyName(AssemblyName name)
        {
            try
            {
                name.VersionCompatibility = AssemblyVersionCompatibility.SameProcess;
                name.Version = new Version(1, 0, 0, 0);
                name.CultureInfo = CultureInfo.InvariantCulture;
                name.Flags = AssemblyNameFlags.EnableJITcompileTracking | AssemblyNameFlags.EnableJITcompileOptimizer;
                name.HashAlgorithm = AssemblyHashAlgorithm.None;
                name.ProcessorArchitecture = ProcessorArchitecture.X86;
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to modify assembly name ({name.FullName}), attempting to load anyways .. ({ex.Message})");
            }
        }

        private static bool ValidatePlugin(Type type, 
            
            out object instance, 
            
            out Action reload, 
            out Action unload, 
            out Action load)
        {
            instance = null;
            reload = null;
            unload = null;
            load = null;

            if (!type.IsStatic())
            {
                instance = InstanceManager.Get(type);

                if (instance is null)
                {
                    Log.Error($"Failed to create instance of type '{type.FullName}'");
                    return false;
                }
            }

            var loadMethod = type.GetMethod("Load", MethodExtensions.BindingFlags);

            if (loadMethod is null)
            {
                Log.Error($"Type '{type.FullName}' (which is marked as a plugin of assembly '{type.Assembly.GetName().Name}') is missing the \"Load\" method! Skipping plugin load.");
                return false;
            }

            var reloadMethod = type.GetMethod("Reload", MethodExtensions.BindingFlags);
            var unloadMethod = type.GetMethod("Unload", MethodExtensions.BindingFlags);

            if (!loadMethod.TryCreateDelegate(instance, out load))
            {
                Log.Error($"Failed to create a LOAD delegate of method '{loadMethod.ToName()}', skipping plugin load.");
                return false;
            }

            if (reloadMethod != null && !reloadMethod.TryCreateDelegate(instance, out reload))
                Log.Error($"Failed to create a RELOAD delegate of method '{unloadMethod.ToName()}'");

            if (unloadMethod != null && !unloadMethod.TryCreateDelegate(instance, out unload))
                Log.Warn($"Failed to create a UNLOAD delegate of method '{unloadMethod.ToName()}'");

            return true;
        }
    }
}