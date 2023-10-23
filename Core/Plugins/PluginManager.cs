using Compendium.Assemblies;
using Compendium.Compilation;
using Compendium.Logging;
using Compendium.Packaging;
using Compendium.Plugins.Packaging;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Compendium.Plugins
{
    public static class PluginManager
    {
        public const string AssemblyExtension = ".dll";
        public const string PackageExtension = ".plpkg";

        private static readonly List<PluginDescriptor> _plugins = new List<PluginDescriptor>();
        private static readonly List<AssemblyReference> _dependencies = new List<AssemblyReference>();

        public static IReadOnlyList<PluginDescriptor> Plugins => _plugins;
        public static IReadOnlyList<AssemblyReference> Dependencies => _dependencies;

        static PluginManager()
        {
            Packager.SetReaderWriter<PackagedAssembly, AssemblyPackager, AssemblyPackager>();
            Packager.SetReaderWriter<PackagedPlugin, PluginPackager, PluginPackager>();
        }

        public static void Search(string directory)
        {
            foreach (var dir in Directory.GetDirectories(directory))
            {
                if (Directory.GetFiles(dir).Any(f => f.EndsWith(".cs")))
                {
                    Log.Info("Plugin Manager", $"Found a source code folder at '{dir}', attempting to compile ..");

                    Compiler.Compile(dir, result =>
                    {
                        Log.Info("Compiler", $"Compilation finished with {result.Warnings.Length} warning(s) and {result.Errors.Length} error(s).");

                        if (result.IsCompiled && result.Result != null && result.Raw != null)
                        {
                            if (result.Warnings != null && result.Warnings.Length > 0)
                            {
                                for (int i = 0; i < result.Warnings.Length; i++)
                                    Log.Warn($"Compiler < {result.Warnings[i].Code} >", $"{result.Warnings[i].Message} (file: '{result.Warnings[i].File}')");
                            }

                            Log.Info("Plugin Manager", $"Loading compiled assembly ..");

                            LoadAndPack(result.Result, result.Raw);
                        }
                        else
                        {
                            Log.Error("Plugin Manager", $"Compilation of '{dir}' failed!");

                            if (result.Errors != null && result.Errors.Length > 0)
                            {
                                for (int i = 0; i < result.Errors.Length; i++)
                                    Log.Error($"Compiler < {result.Errors[i].Code} >", $"{result.Errors[i].Message} (file: '{result.Errors[i].File}')");
                            }

                            if (result.Warnings != null && result.Warnings.Length > 0)
                            {
                                for (int i = 0; i < result.Warnings.Length; i++)
                                    Log.Warn($"Compiler < {result.Warnings[i].Code} >", $"{result.Warnings[i].Message} (file: '{result.Warnings[i].File}')");
                            }
                        }
                    });
                }
            }

            foreach (var path in Directory.GetFiles(directory))
            {
                if (path.EndsWith(PackageExtension))
                    Unpack(path);
                else if (path.EndsWith(AssemblyExtension))
                    LoadAndPack(path);
            }
        }

        public static void Unpack(string packagePath)
        {
            var packageBytes = IO.File.Read(packagePath);
            var package = Packager.ReadPackage<PackagedPlugin>(packageBytes);

            LoadPackaged(package);
        }

        public static void LoadAndPack(string assemblyPath)
            => LoadAndPack(IO.File.Read(assemblyPath));

        public static void LoadAndPack(byte[] bytes)
        {

        }

        public static void LoadAndPack(Assembly assembly, byte[] source)
        {

        }

        public static void LoadPackaged(PackagedPlugin packagedPlugin)
        {
            for (int i = 0; i < packagedPlugin.Dependencies.Length; i++)
                _dependencies.Add(LoadPackaged(packagedPlugin.Dependencies[i]));

            var pluginAssemblyRef = LoadPackaged(packagedPlugin.Assembly);


        }

        public static AssemblyReference LoadPackaged(PackagedAssembly packagedAssembly)
        {

        }
    }
}