using System.IO;

namespace Compendium.Packaging
{
    public abstract class PackageReader
    {
        public abstract PackageData Read(BinaryReader reader);
    }
}