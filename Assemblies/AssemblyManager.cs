using System;
using System.Collections.Generic;

namespace Compendium.Assemblies
{
    public static class AssemblyManager
    {
        public static event Action<AssemblyInfo> OnAssemblyResolved;

        public static AssemblyInfo Assembly { get; } = new AssemblyInfo(null, null, true, null, null);

        public static List<AssemblyInfo> ResolveAssemblies(string directory, AssemblyResolveFlags flags = AssemblyResolveFlags.None) { }

        public static AssemblyInfo ResolveAssembly(string path, AssemblyResolveFlags flags = AssemblyResolveFlags.None) { }
        public static AssemblyInfo ResolveAssembly(byte[] bytes, AssemblyResolveFlags flags = AssemblyResolveFlags.None, string originalPath = null) { }

        public static AssemblyDependencyInfo ResolveDependencies(byte[] bytes, AssemblyResolveFlags flags = AssemblyResolveFlags.None) { }
    }
}