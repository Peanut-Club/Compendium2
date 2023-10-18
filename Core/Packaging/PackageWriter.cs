using System.IO;

namespace Compendium.Packaging
{
    public abstract class PackageWriter
    {
        public abstract void Write(BinaryWriter writer, PackageData data);
    }
}