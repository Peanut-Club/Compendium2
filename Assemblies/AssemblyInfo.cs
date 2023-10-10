using System.Reflection;

namespace Compendium.Assemblies
{
    public class AssemblyInfo
    {
        public string Path { get; }

        public byte[] Bytes { get; }

        public bool IsLoaded { get; }

        public Assembly Assembly { get; }
        public AssemblyDependencyInfo DependencyInfo { get; }

        public AssemblyInfo(string path, byte[] bytes, bool isLoaded, Assembly assembly, AssemblyDependencyInfo dependencyInfo)
        {
            Path = path;
            Bytes = bytes;
            IsLoaded = isLoaded;
            Assembly = assembly;
            DependencyInfo = dependencyInfo;
        }
    }
}