using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Compendium.Assemblies
{
    public static class AssemblyUtils
    {
        public static void RefreshBytes(AssemblyDefinition def, ref byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                def.Write(stream);

                bytes = stream.ToArray();
            }
        }

        public static AssemblyDefinition ReadDefinition(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return AssemblyDefinition.ReadAssembly(stream);
        }

        public static ModuleDefinition[] GatherModules(AssemblyDefinition assemblyDefinition)
        {
            var list = new List<ModuleDefinition>();

            if (assemblyDefinition.MainModule != null)
                list.Add(assemblyDefinition.MainModule);

            foreach (var mod in assemblyDefinition.Modules)
            {
                if (!list.Contains(mod))
                    list.Add(mod);
            }

            return list.ToArray();
        }

        public static Assembly GetAssembly(AssemblyNameReference assemblyNameReference) => null;

        public static bool IsAvailable(AssemblyNameReference assemblyNameReference)
            => GetAssembly(assemblyNameReference) != null;

        public static AssemblyDependency[] GetDependencies(IEnumerable<AssemblyNameReference> all, IEnumerable<AssemblyNameReference> missing)
        {
            var deps = new List<AssemblyDependency>();

            foreach (var asmRef in all)
            {
                if (missing.Contains(asmRef))
                    deps.Add(
                        new AssemblyDependency(
                            AssemblyDependencyStatus.Failed,
                            AssemblySource.Raw,

                            null,

                            asmRef,

                            null,

                            asmRef.FullName,

                            asmRef.PublicKeyToken != null ? BitConverter.ToString(asmRef.PublicKeyToken) : "",

                            asmRef.Culture ?? "",
                            asmRef.Version?.ToString() ?? "0.0.0.0"));
                else
                    deps.Add(
                        new AssemblyDependency(
                            AssemblyDependencyStatus.Loaded,
                            AssemblySource.Raw,
                            
                            GetAssembly(asmRef),

                            asmRef,

                            null,

                            asmRef.FullName,

                            asmRef.PublicKeyToken != null ? BitConverter.ToString(asmRef.PublicKeyToken) : "",

                            asmRef.Culture ?? "",
                            asmRef.Version?.ToString() ?? "0.0.0.0"));
            }

            return deps.ToArray();
        }
    }
}