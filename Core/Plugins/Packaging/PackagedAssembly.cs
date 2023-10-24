using Compendium.Packaging;

namespace Compendium.Plugins.Packaging
{
    public struct PackagedAssembly : IPackage
    {
        public string Name;
        public byte[] Data;
    }
}