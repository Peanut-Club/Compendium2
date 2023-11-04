using Compendium.Logging;
using Compendium.Plugins;

using Mono.Cecil;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace Compendium.Assemblies
{
    public class LoadedAssembly
    {
        public Assembly Assembly { get; }

        public Assembly[] Dependencies { get; }

        public AssemblyNameReference[] References { get; }
        public AssemblyNameReference[] Missing { get; }

        public LoadedAssembly(Assembly assembly, Assembly[] dependencies, AssemblyNameReference[] references, AssemblyNameReference[] missing)
        {
            Assembly = assembly;
            Dependencies = dependencies;
            References = references;
            Missing = missing;
        }

        public static LoadedAssembly Load(byte[] bytes)
        {
            var resolved = new List<Assembly>();
            var all = new List<AssemblyNameReference>();
            var missing = new List<AssemblyNameReference>();

            using (var ms = new MemoryStream(bytes))
            using (var asm = AssemblyDefinition.ReadAssembly(ms))
            {
                if (asm.Modules is null || asm.Modules.Count <= 0)
                {
                    Log.Error("Loaded Assembly", $"Cannot load assembly '{asm.FullName}' - assembly doesn't contain any modules.");
                    return null;
                }

                var module = asm.Modules[0];
                var refs = module.AssemblyReferences.ToArray();

                for (int i = 0; i < refs.Length; i++)
                {
                    var asmRef = refs[i];

                    asmRef.Version = null;
                    asmRef.Culture = null;

                    all.Add(asmRef);

                    Log.Trace("Loaded Assembly", $"Removed checks for dependency '{asmRef.FullName}'");

                    if (!Resolve(asmRef, out var depAssembly))
                        missing.Add(asmRef);
                    else
                        resolved.Add(depAssembly);
                }

                using (var rMs = new MemoryStream())
                {
                    asm.Write(rMs);
                    bytes = rMs.ToArray();
                }
            }

            var assembly = Assembly.Load(bytes);
            var embeds = ResolveEmbeds(assembly);

            missing.RemoveAll(asmRef => embeds.Any(asm => asm.FullName == asmRef.FullName));

            resolved.AddRange(embeds);

            return new LoadedAssembly(assembly, resolved.ToArray(), all.ToArray(), missing.ToArray());
        }

        private static bool Resolve(AssemblyNameReference asmRef, out Assembly assembly)
        {
            foreach (var dep in PluginManager.Dependencies)
            {
                if (dep.FullName == asmRef.FullName)
                {
                    assembly = dep;
                    return true;
                }
            }

            assembly = null;
            return false;
        }

        private static Assembly[] ResolveEmbeds(Assembly assembly)
        {
            var list = new List<Assembly>();
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var name in resourceNames)
            {
                if (name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    using (var ms = new MemoryStream())
                    using (var ds = assembly.GetManifestResourceStream(name))
                    {
                        ds.CopyTo(ms);

                        var embedDep = Assembly.Load(ms.ToArray());

                        if (embedDep != null)
                        {
                            Log.Trace("Loaded Assembly", $"Resolved embedded assembly: {embedDep.FullName}");
                            list.Add(embedDep);
                        }
                    }
                }
                else if (name.EndsWith(".dll.compressed", StringComparison.OrdinalIgnoreCase))
                {
                    using (var ds = assembly.GetManifestResourceStream(name))
                    using (var dfs = new DeflateStream(ds, CompressionMode.Decompress))
                    using (var ms = new MemoryStream())
                    {
                        dfs.CopyTo(ms);

                        var embedDep = Assembly.Load(ms.ToArray());

                        if (embedDep != null)
                        {
                            Log.Trace("Loaded Assembly", $"Resolved embedded assembly: {embedDep.FullName}");
                            list.Add(embedDep);
                        }
                    }
                }
            }

            return list.ToArray();
        }
    }
}