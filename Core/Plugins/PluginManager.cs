using Compendium.Logging;
using Compendium.Logging.Formatting;
using Compendium.Logging.Streams.Console;
using Compendium.Logging.Streams.File;
using Compendium.Logging.Streams.Unity;
using Compendium.Utilities.Reflection;
using Compendium.Extensions;
using Compendium.Callbacks;
using Compendium.Assemblies;
using Compendium.IO;

using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace Compendium.Plugins
{
    public static class PluginManager
    {
        private static List<Assembly> PrivateDeps;
        private static List<Plugin> PrivatePlugins;

        public static Log Logger { get; }

        public static IReadOnlyList<Assembly> Dependencies => PrivateDeps;
        public static IReadOnlyList<Plugin> Plugins => PrivatePlugins;

        static PluginManager()
        {
            Logger = new Log(10, 20, 5, LogTypes.LowDebugging, LogFormatting.Source | LogFormatting.Message, "Plugin Manager");

            Logger.AddStream<ConsoleStream>();
            Logger.AddStream<UnityStream>();
            Logger.AddStream(new FileLogStream(FileLogStreamMode.Interval, Paths.GetPath("plugin_manager_log.txt", "pluginManagerLog", PathType.Log), 1500));

            PrivateDeps = new List<Assembly>();
            PrivatePlugins = new List<Plugin>();

            PrivateDeps.AddRange(AppDomain.CurrentDomain.GetAssemblies());
        }

        [LoadCallback]
        public static void Load()
        {
            Logger.Info("Loading ...");

            Logger.Info("Loading dependencies ..");

            foreach (var file in Paths.GetDirectory("pluginDependencies", Paths.DependencyDirectory, Paths.GlobalDependencyDirectory).GetFilePaths())
            {
                try
                {
                    var asmBytes = File.Read(file);

                    if (asmBytes.Length <= 0)
                    {
                        Logger.Error($"Cannot load file '{file}': image is too short");
                        continue;
                    }

                    var asm = Assembly.Load(asmBytes);

                    if (asm is null)
                    {
                        Logger.Error($"Cannot load file '{file}'");
                        continue;
                    }

                    if (PrivateDeps.Any(p => p.FullName == asm.FullName))
                    {
                        Logger.Warn($"Dependency '{asm.FullName}' is already loaded.");
                        continue;
                    }

                    PrivateDeps.Add(asm);

                    Logger.Info($"Loaded dependency: '{asm.GetName().Name}'");
                }
                catch (Exception ex)
                {
                    Logger.Error($"An exception occured while resolving assembly file '{file}'", ex);
                }
            }

            Logger.Info($"Loaded {PrivateDeps.Count} dependencies");

            Logger.Info("Loading plugins ..");

            foreach (var file in Paths.GetDirectory("plugins", Paths.PluginsDirectory, Paths.GlobalPluginsDirectory).GetFilePaths())
            {
                var name = System.IO.Path.GetFileNameWithoutExtension(file);

                try
                {
                    var asmBytes = File.Read(file);

                    if (asmBytes.Length <= 0)
                    {
                        Logger.Error($"Cannot load file '{file}': image is too short");
                        continue;
                    }

                    var asm = LoadedAssembly.Load(asmBytes);

                    if (asm is null)
                    {
                        Logger.Error($"Cannot load file '{file}'");
                        continue;
                    }

                    if (asm.Missing.Length > 0)
                    {
                        Logger.Warn($"Plugin '{name}' contains missing dependencies!");

                        for (int i = 0; i < asm.Missing.Length; i++)
                            Logger.Warn($"{i + 1}) {asm.Missing[i].FullName}");

                        continue;
                    }

                    PrivateDeps.AddRange(asm.Dependencies.Where(d => !PrivateDeps.Contains(d)));

                    if (asm.Assembly is null)
                    {
                        Logger.Error($"Failed to load plugin file '{name}'");
                        continue;
                    }

                    LoadAssembly(asm);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load plugin file '{name}' due to an exception", ex);
                }
            }
        }

        public static void LoadAssembly(LoadedAssembly loadedAssembly)
        {
            var asm = loadedAssembly.Assembly;

            foreach (var type in asm.Types())
            {

            }
        }
    }
}