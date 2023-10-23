using System.Reflection;

namespace Compendium.Compilation
{
    public struct CompilationResult
    {
        public CompilationMessage[] Errors;
        public CompilationMessage[] Warnings;
        public Assembly Result;
        public byte[] Raw;
        public bool IsCompiled;
    }
}