using Compendium.Packaging;

namespace Compendium.Plugins.Packaging
{
    public struct PackagedAssembly : IPackage
    {
        public string Name;
        public string Culture;
        public string Token;
        public string Version;

        public byte[] Data;
    }
}