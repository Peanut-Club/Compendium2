using Mono.Cecil;

using System.Reflection;

namespace Compendium.Assemblies
{
    public struct AssemblyReference
    {
        public Assembly Assembly;
        public AssemblyDefinition Definition;
        public AssemblyReference[] Dependencies;
    }
}