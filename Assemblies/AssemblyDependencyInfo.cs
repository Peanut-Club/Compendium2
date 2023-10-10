using System.Reflection;

namespace Compendium.Assemblies
{
    public class AssemblyDependencyInfo
    {
        public Assembly[] Dependencies { get; }

        public AssemblyName[] Missing { get; }
        public AssemblyName[] Mismatched { get; }
        public AssemblyName[] Loaded { get; }

        public AssemblyDependencyInfo(
            Assembly[] dependencies,
            AssemblyName[] missing,
            AssemblyName[] mismatched,
            AssemblyName[] loaded)
        {
            Dependencies = dependencies;
            Missing = missing;
            Mismatched = mismatched;
            Loaded = loaded;
        }
    }
}