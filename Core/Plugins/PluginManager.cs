using Compendium.Packaging;
using Compendium.Plugins.Packaging;

using System.Collections.Generic;
using System.Linq;
using System.IO;

using Utils.NonAllocLINQ;

using Compendium.IO;
using Compendium.Assemblies;
using Compendium.Extensions;
using Compendium.Logging;
using Compendium.Logging.Formatting;
using Compendium.Logging.Streams.Console;
using Compendium.Logging.Streams.Unity;
using Compendium.Logging.Streams.File;
using Compendium.Compilation;
using Compendium.Utilities;
using Compendium.Pooling.Pools;

using File = Compendium.IO.File;

namespace Compendium.Plugins
{
    public static class PluginManager
    {
        public const string AssemblyExtension = ".dll";
        public const string PackageExtension = ".plpkg";

        private static readonly List<PluginDescriptor> _plugins = new List<PluginDescriptor>();
        private static readonly List<AssemblyDependency> _dependencies = new List<AssemblyDependency>();

        public static IReadOnlyList<PluginDescriptor> Plugins => _plugins;
        public static IReadOnlyList<AssemblyDependency> Dependencies => _dependencies;

        public static Log Logger { get; } 

        public static string DependencyPackagePath { get; } = Paths.GetPath("dependency_package.deps", "dependencyPackage", PathType.Cache);

        static PluginManager()
        {
            Logger = new Log(10, 10, 5, LogTypes.LowDebugging, LogFormatting.Source | LogFormatting.Message, "Plugin Manager");

            Logger.AddStream<ConsoleStream>();
            Logger.AddStream<UnityStream>();

            Logger.AddStream(new FileLogStream(FileLogStreamMode.Interval, Paths.GetPath("plugin_manager_log", "pluginManagerLog", PathType.Log), 1500));

            Packager.SetReaderWriter<PackagedAssembly, AssemblyPackager, AssemblyPackager>();
        }

        public static void Search(string directory)
        {
            ReloadDependencyPackage();

            Logger.Trace($"Searching directory: {directory}");

            var dirsToDelete = ListPool<string>.Shared.Rent();
            var filesToDelete = ListPool<string>.Shared.Rent();

            var dirs = System.IO.Directory.GetDirectories(directory);
            var files = System.IO.Directory.GetFiles(directory);

            for (int i = 0; i < dirs.Length; i++)
            {
                Logger.Trace($"Searching subdirectory: {dirs[i]}");

                var subFiles = System.IO.Directory.GetFiles(dirs[i]);

                if (subFiles.Length <= 0)
                {
                    Logger.Trace("Subdirectory doesnt contain any files");
                    continue;
                }

                if (subFiles.Any(f => f.EndsWith(".cs")))
                {
                    Logger.Trace("Subdirectory contains project files, attempting to compile.");

                    Compiler.Compile(dirs[i], result =>
                    {
                        if (!result.IsCompiled)
                        {
                            Logger.Error($"Compilation of directory '{Path.GetDirectoryName(dirs[i])}' failed.");

                            for (int i = 0; i < result.Errors.Length; i++)
                                Logger.Error($"Compiler Error [{result.Errors[i].Code}]: {result.Errors[i].Message} (in: {result.Errors[i].File})");
                        }
                        else
                        {
                            Logger.Info($"Compilation of directory '{Path.GetDirectoryName(dirs[i])}' has finished with {result.Warnings.Length} warning(s).");

                            for (int i = 0; i < result.Warnings.Length; i++)
                                Logger.Warn($"Compiler Warning [{result.Warnings[i].Code}]: {result.Warnings[i].Message} (in: {result.Warnings[i].File})");

                            if (!WritePackage(result.Raw, out var package))
                            {
                                Logger.Error($"Failed to load compiler result: failed to create package.");
                                return;
                            }

                            if (!DependencyUtils.LoadFromPackage(package, true,
                                out var asmRef,
                                out var asmDeps,
                                out var asmMissingDeps))
                            {
                                Logger.Error($"Failed to compiler result: failed to load package.");
                                return;
                            }

                            if (asmMissingDeps.Length > 0)
                            {
                                Logger.Error($"Failed to load compiler result: missing dependencies.");

                                for (int y = 0; y < asmMissingDeps.Length; y++)
                                    Logger.Error(asmMissingDeps[y].FullName);

                                return;
                            }

                            for (int y = 0; y < asmDeps.Length; y++)
                                _dependencies.AddIfNotContains(asmDeps[y]);

                            LoadPlugin(asmRef);
                        }
                    });
                }
                else
                {
                    Logger.Trace("Subdirectory doesnt contain any project files, scheduling for deletion");
                    dirsToDelete.Add(dirs[i]);
                }
            }

            Logger.Trace($"Found {files.Length} in directory");

            for (int x = 0; x < files.Length; x++)
            {
                if (files[x].EndsWith(AssemblyExtension))
                {
                    if (!WritePackage(files[x], out var package))
                    {
                        Logger.Error($"Failed to load file '{Path.GetFileName(files[x])}': failed to create package.");
                        continue;
                    }

                    if (!DependencyUtils.LoadFromPackage(package, true,
                        out var asmRef,
                        out var asmDeps,
                        out var asmMissingDeps))
                    {
                        Logger.Error($"Failed to load file '{Path.GetFileName(files[x])}': failed to load package.");
                        continue;
                    }

                    if (asmMissingDeps.Length > 0)
                    {
                        Logger.Error($"Failed to load file '{Path.GetFileName(files[x])}': missing dependencies.");

                        for (int y = 0; y < asmMissingDeps.Length; y++)
                            Logger.Error(asmMissingDeps[y].FullName);

                        continue;
                    }

                    for (int y = 0; y < asmDeps.Length; y++)
                        _dependencies.AddIfNotContains(asmDeps[y]);

                    LoadPlugin(asmRef);
                }
                else if (files[x].EndsWith(PackageExtension))
                {
                    var packageData = File.Read(files[x]);
                    var package = Packager.ReadPackage<PackagedAssembly>(packageData);

                    if (!DependencyUtils.LoadFromPackage(package, true,
                        out var asmRef,
                        out var asmDeps,
                        out var asmMissingDeps))
                    {
                        Logger.Error($"Failed to load file '{Path.GetFileName(files[x])}': failed to load package.");
                        continue;
                    }

                    if (asmMissingDeps.Length > 0)
                    {
                        Logger.Error($"Failed to load file '{Path.GetFileName(files[x])}': missing dependencies.");

                        for (int y = 0; y < asmMissingDeps.Length; y++)
                            Logger.Error(asmMissingDeps[y].FullName);

                        continue;
                    }

                    for (int y = 0; y < asmDeps.Length; y++)
                        _dependencies.AddIfNotContains(asmDeps[y]);

                    LoadPlugin(asmRef);
                }
                else
                    Logger.Trace($"Unrecognized file: '{Path.GetFileName(files[x])}', skipping");
            }

            WriteDependencyPackage();

            foreach (var file in filesToDelete)
                System.IO.File.Delete(file);

            foreach (var dir in dirsToDelete)
                System.IO.Directory.Delete(dir, true);

            ListPool<string>.Shared.Return(filesToDelete);
            ListPool<string>.Shared.Return(dirsToDelete);
        }

        private static void LoadPlugin(AssemblyReference assemblyReference)
        {
            
        }

        private static bool WritePackage(string filePath, out PackagedAssembly packagedAssembly)
            => WritePackage(File.Read(filePath), out packagedAssembly);

        private static bool WritePackage(byte[] data, out PackagedAssembly packagedAssembly) 
        { 
            if (!DependencyUtils.LoadFromRaw(data, true, 
                out var asmPackage,

                out _,
                out _,
                out _))
            {
                Logger.Error($"Failed to load raw: failed to load package.");

                packagedAssembly = default;
                return false;
            }

            packagedAssembly = asmPackage;
            return true;
        }

        private static void WriteDependencyPackage()
        {
            Logger.Trace("Refreshing the dependency package ..");

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.WriteArray(_dependencies.ToArray(), (bw, dep) =>
                {
                    var bytes = Packager.WritePackage(new PackagedAssembly
                    {
                        Data = dep.Raw,
                        Name = dep.Name
                    });

                    bw.WriteArray(bytes, (b, bv) => b.Write(bv));
                });

                File.Write(DependencyPackagePath, ms.ToArray());
            }
        }

        private static void ReloadDependencyPackage()
        {
            if (!System.IO.File.Exists(DependencyPackagePath))
                return;

            var packageBytes = File.Read(DependencyPackagePath);

            if (packageBytes.Length <= 0)
                return;

            Logger.Trace($"Read {packageBytes.Length} bytes from dependency package '{DependencyPackagePath}'");

            using (var ms = new MemoryStream(packageBytes))
            using (var br = new BinaryReader(ms))
            {
                var assemblyArray = br.ReadArray<PackagedAssembly>(br =>
                {
                    var packageData = br.ReadArray<byte>(b => b.ReadByte());

                    return Packager.ReadPackage<PackagedAssembly>(packageData);
                });

                if (assemblyArray.Length <= 0)
                    Logger.Trace("Failed to read any assemblies from the dependency package");
                else
                {
                    for (int i = 0; i < assemblyArray.Length; i++)
                    {
                        if (!DependencyUtils.LoadDepFromPackage(assemblyArray[i], true, out var depInfo))
                        {
                            Logger.Error($"Failed to load dependency '{assemblyArray[i].Name}' from dependency package.");
                            continue;
                        }

                        if (depInfo.Assembly is null)
                        {
                            Logger.Error($"Failed to load dependency assembly '{assemblyArray[i].Name}' from dependency package.");
                            continue;
                        }

                        _dependencies.AddIfNotContains(depInfo);

                        Logger.Trace($"Loaded dependency '{assemblyArray[i].Name}' from the dependency package.");
                    }
                }
            }
        }
    }
}