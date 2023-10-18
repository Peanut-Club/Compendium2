using Mono.Cecil;

using System.Reflection;

namespace Compendium.Assemblies
{
    public class AssemblyDependency
    {
        public AssemblyDependencyStatus Status { get; }
        public AssemblySource Source { get; }

        public Assembly Assembly { get; }
        public AssemblyNameReference Reference { get; }

        public string Path { get; }

        public string RequiredVersion { get; }
        public string RequiredToken { get; }
        public string RequiredCulture { get; }
        public string RequiredName { get; }

        public AssemblyDependency(AssemblyDependencyStatus assemblyDependencyStatus, AssemblySource source, Assembly assembly, AssemblyNameReference reference, string path, string requiredName, string requiredToken, string requiredCulture, string requiredVersion)
        {
            Status = assemblyDependencyStatus;
            Source = source;
            Assembly = assembly;
            Reference = reference;
            Path = path;

            RequiredName = requiredName;
            RequiredToken = requiredToken;
            RequiredCulture = requiredCulture;
            RequiredVersion = requiredVersion;
        }
    }
}