using Compendium.Packaging;

namespace Compendium.Plugins.Packaging
{
    public struct PackagedPlugin : IPackage
    {
        public PluginMetadata Metadata;
        public PluginSymbols Symbols;

        public PackagedAssembly Assembly;
        public PackagedAssembly[] Dependencies;

        public string PackageHash;
    }
}