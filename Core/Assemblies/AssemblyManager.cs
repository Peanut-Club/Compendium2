using Compendium.Assemblies.Packaging;
using Compendium.Packaging;
using Compendium.Utilities;

using Mono.Cecil;

using MonoMod.Utils;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compendium.Assemblies
{
    public static class AssemblyManager
    {
        public static AssemblyInfo MainAssembly { get; }

        public static event Action<AssemblyInfo> OnRead;

        static AssemblyManager()
        {
            Packager.SetReaderWriter<AssemblyPackageData, AssemblyPackageWriter, AssemblyPackageReader>();

            MainAssembly = ReadAssembly(Assembly.GetExecutingAssembly(), null, IO.File.Read(Assembly.GetExecutingAssembly().Location));
        }

        public static AssemblyInfo[] ReadDirectory(string directory, AssemblyReaderFlags flags = AssemblyReaderFlags.None)
        {
            if (string.IsNullOrWhiteSpace(directory))
                throw new ArgumentNullException(nameof(directory));

            var list = new List<AssemblyInfo>();

            foreach (var file in System.IO.Directory.GetFiles(directory))
            {
                if (!file.EndsWith(".dll") && !file.EndsWith(".asmpkg"))
                    continue;

                var asm = ReadFile(file, flags);

                if (asm != null)
                    list.Add(asm);
            }

            return list.ToArray();
        }

        public static AssemblyInfo ReadFile(string path, AssemblyReaderFlags flags = AssemblyReaderFlags.None)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (path.EndsWith(".asmpkg"))
                return ReadPackage(Packager.ReadPackage<AssemblyPackageData>(IO.File.Read(path)), flags, path);
            else if (path.EndsWith(".dll"))
                return ReadRaw(IO.File.Read(path), flags, path);
            else
                return null;
        }

        public static AssemblyInfo ReadRaw(byte[] bytes, AssemblyReaderFlags flags = AssemblyReaderFlags.None, string origPath = null)
        {
            var def = AssemblyUtils.ReadDefinition(bytes);

            if (def is null)
                return null;

            var mods = AssemblyUtils.GatherModules(def);

            if (mods is null || mods.Length <= 0)
                return null;

            var missing = new List<AssemblyNameReference>();
            var all = new List<AssemblyNameReference>();

            for (int i = 0; i < mods.Length; i++)
            {
                foreach (var asmRef in mods[i].AssemblyReferences)
                {
                    all.Add(asmRef);

                    if (asmRef.IsWindowsRuntime)
                        continue;

                    if (flags.Any(AssemblyReaderFlags.IgnoreCulture))
                        asmRef.Culture = null;

                    if (flags.Any(AssemblyReaderFlags.IgnoreVersion))
                        asmRef.Version = null;

                    if (flags.Any(AssemblyReaderFlags.IgnoreToken))
                    {
                        asmRef.PublicKey = null;
                        asmRef.PublicKeyToken = null;
                        asmRef.HasPublicKey = false;
                    }

                    if (!AssemblyUtils.IsAvailable(asmRef))
                        missing.Add(asmRef);
                }
            }
            
            AssemblyUtils.RefreshBytes(def, ref bytes);

            if (missing.Count > 0)
                return new AssemblyInfo(
                    AssemblyStatus.MissingDependencies, 
                    
                    null, 
                    def,
                    
                    AssemblyUtils.GetDependencies(all, missing), 
                    
                    AssemblySource.Raw, 
                    
                    origPath);

            var assembly = Assembly.Load(bytes);

            if (assembly is null)
                return null;

            return ReadAssembly(assembly);
        }

        public static AssemblyInfo ReadPackage(AssemblyPackageData assemblyPackage, AssemblyReaderFlags flags = AssemblyReaderFlags.None, string path = null)
        {
            for (int i = 0; i < assemblyPackage.RawDependencies.Length; i++)
                ReadRaw(assemblyPackage.RawDependencies[i], flags, path);

            return ReadRaw(assemblyPackage.RawAssembly, flags, path);
        }

        public static AssemblyInfo ReadAssembly(Assembly assembly, AssemblyDefinition assemblyDefinition = null, byte[] bytes = null)
        {
            assemblyDefinition ??= AssemblyUtils.ReadDefinition(bytes);

            var mods = AssemblyUtils.GatherModules(assemblyDefinition);

            var deps = new List<AssemblyNameReference>();
            var missing = new List<AssemblyNameReference>();

            mods.ForEach(mod =>
            {
                foreach (var asmRef in mod.AssemblyReferences)
                {
                    if (!deps.Contains(asmRef))
                        deps.Add(asmRef);

                    if (!AssemblyUtils.IsAvailable(asmRef))
                        missing.Add(asmRef);
                }
            });

            return new AssemblyInfo(AssemblyStatus.Loaded, assembly, assemblyDefinition, AssemblyUtils.GetDependencies(deps, missing), AssemblySource.Direct, assembly.Location);
        }

        public static AssemblyPackageData WritePackage(Assembly assembly, byte[] rawAssembly, byte[][] rawDeps)
        {
            var version = assembly.GetName().Version;

            return new AssemblyPackageData
            {
                Author = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Unknown",
                Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "Unknown",
                Hash = assembly.GetRuntimeHashedFullName(),
                Id = RandomGeneration.String(),
                Name = assembly.GetName().Name,

                RawAssembly = rawAssembly,
                RawDependencies = rawDeps,

                VersionMajor = version.Major,
                VersionMinor = version.Minor,
                VersionPatch = version.Build
            };
        }
    }
}