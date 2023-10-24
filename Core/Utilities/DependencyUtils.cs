using Compendium.Assemblies;
using Compendium.Plugins;
using Compendium.Plugins.Packaging;
using Compendium.Pooling.Pools;

using Mono.Cecil;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Compendium.Utilities
{
    public static class DependencyUtils
    {
        private static readonly Dictionary<AssemblyNameReference, Tuple<Version, string, byte[]>> _originalValues = new Dictionary<AssemblyNameReference, Tuple<Version, string, byte[]>>();

        public static readonly ReaderParameters readerParameters = new ReaderParameters(ReadingMode.Immediate);
        public static readonly WriterParameters writerParameters = new WriterParameters();

        public static AssemblyDependency ToDependency(AssemblyNameReference reference, byte[] data, bool removedRefs)
            => new AssemblyDependency
            {
                Assembly = Assembly.Load(data),

                Name = reference.FullName,

                Reference = reference,
                Raw = data,
                RemovedRefs = removedRefs
            };

        public static AssemblyDependency ToMissingDependency(AssemblyNameReference reference)
            => new AssemblyDependency
            {
                Assembly = null,
                Raw = null,

                Reference = reference,

                RemovedRefs = false,

                Name = reference.FullName      
            };

        public static bool TryResolve(AssemblyNameReference reference, out AssemblyDependency dependency)
        {
            for (int i = 0; i < PluginManager.Dependencies.Count; i++)
            {
                if (PluginManager.Dependencies[i].Reference == reference)
                {
                    dependency = PluginManager.Dependencies[i];
                    return true;
                }
            }

            dependency = default;
            return false;
        }

        public static bool LoadDepFromPackage(PackagedAssembly package, bool removeRefChecks, out AssemblyDependency assemblyReference)
        {
            if (!LoadDefinitionFromRaw(package.Data, out var definition))
            {
                assemblyReference = default;
                return false;
            }

            var mods = GetModules(definition);
            var refs = GetReferences(mods);

            if (removeRefChecks)
            {
                for (int i = 0; i < refs.Length; i++)
                {
                    var asmRef = refs[i];

                    _originalValues[asmRef] = new Tuple<Version, string, byte[]>(asmRef.Version, asmRef.Culture, asmRef.PublicKey);

                    asmRef.Version = null;
                    asmRef.Culture = null;
                    asmRef.HasPublicKey = false;
                    asmRef.PublicKey = null;
                }
            }

            if (removeRefChecks)
                WriteDefinition(definition, ref package.Data);

            assemblyReference = new AssemblyDependency
            {
                Reference = definition.Name,
                Name = definition.Name.FullName,

                Assembly = Assembly.Load(package.Data),

                Raw = package.Data,

                RemovedRefs = removeRefChecks
            };

            return true;
        }

        public static bool LoadFromRaw(byte[] raw, bool removeRefChecks,
            out PackagedAssembly package,
            out AssemblyReference assemblyReference,
            out AssemblyDependency[] assemblyReferences,
            out AssemblyNameReference[] unresolved)
        {
            if (!LoadDefinitionFromRaw(raw, out var definition))
            {
                package = default;
                unresolved = null;
                assemblyReferences = null;
                assemblyReference = default;

                return false;
            }

            var mods = GetModules(definition);
            var refs = GetReferences(mods);

            var deps = ListPool<AssemblyDependency>.Shared.Rent();
            var missing = ListPool<AssemblyNameReference>.Shared.Rent();

            if (removeRefChecks)
            {
                for (int i = 0; i < refs.Length; i++)
                {
                    var asmRef = refs[i];

                    _originalValues[asmRef] = new Tuple<Version, string, byte[]>(asmRef.Version, asmRef.Culture, asmRef.PublicKey);

                    asmRef.Version = null;
                    asmRef.Culture = null;
                    asmRef.HasPublicKey = false;
                    asmRef.PublicKey = null;
                }
            }

            for (int i = 0; i < refs.Length; i++)
            {
                if (TryResolve(refs[i], out var resolved))
                    deps.Add(ToDependency(refs[i], resolved.Raw, removeRefChecks));
                else
                {
                    missing.Add(refs[i]);
                    deps.Add(ToMissingDependency(refs[i]));
                }
            }

            if (removeRefChecks)
                WriteDefinition(definition, ref raw);

            unresolved = ListPool<AssemblyNameReference>.Shared.ToArrayReturn(missing);
            assemblyReferences = ListPool<AssemblyDependency>.Shared.ToArrayReturn(deps);

            assemblyReference = new AssemblyReference
            {
                Definition = definition,
                Name = definition.Name.FullName,

                Assembly = Assembly.Load(raw),

                Raw = raw,

                RemovedRefs = removeRefChecks,
                Dependencies = assemblyReferences,
            };

            package = new PackagedAssembly
            {
                Data = raw,
                Name = definition.Name.FullName
            };

            return true;
        }

        public static bool LoadFromPackage(PackagedAssembly package, bool removeRefChecks, 
            
            out AssemblyReference assemblyReference, 
            out AssemblyDependency[] assemblyReferences, 
            out AssemblyNameReference[] unresolved)
        {
            if (!LoadDefinitionFromRaw(package.Data, out var definition))
            {
                unresolved = null;
                assemblyReferences = null;
                assemblyReference = default;

                return false;
            }

            var mods = GetModules(definition);
            var refs = GetReferences(mods);

            var deps = ListPool<AssemblyDependency>.Shared.Rent();
            var missing = ListPool<AssemblyNameReference>.Shared.Rent();

            if (removeRefChecks)
            {
                for (int i = 0; i < refs.Length; i++)
                {
                    var asmRef = refs[i];

                    _originalValues[asmRef] = new Tuple<Version, string, byte[]>(asmRef.Version, asmRef.Culture, asmRef.PublicKey);

                    asmRef.Version = null;
                    asmRef.Culture = null;
                    asmRef.HasPublicKey = false;
                    asmRef.PublicKey = null;
                }
            }

            for (int i = 0; i < refs.Length; i++)
            {
                if (TryResolve(refs[i], out var resolved))
                    deps.Add(ToDependency(refs[i], resolved.Raw, removeRefChecks));
                else
                {
                    missing.Add(refs[i]);
                    deps.Add(ToMissingDependency(refs[i]));
                }
            }

            if (removeRefChecks)
                WriteDefinition(definition, ref package.Data);

            unresolved = ListPool<AssemblyNameReference>.Shared.ToArrayReturn(missing);
            assemblyReferences = ListPool<AssemblyDependency>.Shared.ToArrayReturn(deps);

            assemblyReference = new AssemblyReference
            {
                Definition = definition,             
                Name = definition.Name.FullName,

                Assembly = Assembly.Load(package.Data),

                Raw = package.Data,

                RemovedRefs = removeRefChecks,
                Dependencies = assemblyReferences,
            };

            return true;
        }

        public static void WriteDefinition(AssemblyDefinition definition, ref byte[] target)
        {
            writerParameters.WriteSymbols = true;

            using (var ms = new MemoryStream())
            {
                definition.Write(ms, writerParameters);
                target = ms.ToArray();
            }
        }

        public static bool LoadDefinitionFromRaw(byte[] raw, out AssemblyDefinition definition)
        {
            using (var ms = new MemoryStream(raw))
                definition = AssemblyDefinition.ReadAssembly(ms, readerParameters);

            return definition != null;
        }

        public static AssemblyNameReference[] GetReferences(ModuleDefinition[] modules)
        {
            var list = ListPool<AssemblyNameReference>.Shared.Rent();

            for (int i = 0; i < modules.Length; i++)
            {
                foreach (var asmRef in modules[i].AssemblyReferences)
                {
                    if (!list.Contains(asmRef))
                        list.Add(asmRef);
                }
            }

            return ListPool<AssemblyNameReference>.Shared.ToArrayReturn(list);
        }

        public static ModuleDefinition[] GetModules(AssemblyDefinition definition)
        {
            var list = ListPool<ModuleDefinition>.Shared.Rent();

            foreach (var mod in definition.Modules)
                list.Add(mod);

            if (definition.MainModule != null && !list.Contains(definition.MainModule))
                list.Add(definition.MainModule);

            return ListPool<ModuleDefinition>.Shared.ToArrayReturn(list);
        }
    }
}