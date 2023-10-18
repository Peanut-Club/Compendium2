using Mono.Cecil;
using System.Reflection;

namespace Compendium.Assemblies
{
    public class AssemblyInfo
    {
        public AssemblyStatus Status { get; }
        public AssemblySource Source { get; }

        public Assembly Assembly { get; }
        public AssemblyDefinition Definition { get; }

        public AssemblyDependency[] Dependencies { get; }

        public string Path { get; }

        public AssemblyInfo(AssemblyStatus status, Assembly assembly, AssemblyDefinition definition, AssemblyDependency[] dependencies, AssemblySource source, string path)
        {
            Status = status;
            Assembly = assembly;
            Definition = definition;
            Dependencies = dependencies;
            Source = source;
            Path = path;
        }
    }
}