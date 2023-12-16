using Common.Logging;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compendium.Plugins
{
    public static class DependencyManager
    {
        private static List<Assembly> deps = new List<Assembly>();

        public static IReadOnlyList<Assembly> Dependencies => deps;

        public static LogOutput Log { get; } = new LogOutput("Dependency Manager");

        public static event Action<Assembly> OnLoaded;
        public static event Action<Assembly> OnUnloaded;

        public static void Load(string directory)
        {

        }

        public static void LoadFile(string filePath)
        {

        }

        public static void LoadAssembly(Assembly assembly, bool isManifest = false)
        {
            if (!isManifest)
            {

            }
            else
            {
                try
                {
                    var resources = assembly.GetManifestResourceNames();

                    for (int i = 0; i < resources.Length; i++)
                    {
                        try
                        {
                            using (var stream = assembly.GetManifestResourceStream(resources[i]))
                            {

                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Failed while loading manifest resource '{resources[i]}' from '{assembly.FullName}':\n{e}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed while loading manifest resources from '{assembly.FullName}':\n{ex}");
                }
            }
        }

        public static bool TryGet(AssemblyName assemblyName, out Assembly assembly)
        {
            for (int i = 0; i < deps.Count; i++)
            {
                var name = deps[i].GetName();

                Log.Trace($"Matching '{name.FullName}' against '{assemblyName.FullName}'");

                if (name.Version == assemblyName.Version 
                    && name.CultureName == assemblyName.CultureName
                    && name.FullName == assemblyName.FullName)
                {
                    Log.Debug($"Found dependency: {name.FullName} (against: {assemblyName.FullName})");

                    assembly = deps[i];
                    return true;
                }
            }

            Log.Debug($"Failed to find dependency '{assemblyName.FullName}'");

            assembly = null;
            return false;
        }
    }
}