using Mono.Cecil;

using System.Reflection;

namespace Compendium.Assemblies
{
    public struct AssemblyDependency
    {
        public byte[] Raw;
        public bool RemovedRefs;
        public string Name;

        public Assembly Assembly;
        public AssemblyNameReference Reference;
    }
}