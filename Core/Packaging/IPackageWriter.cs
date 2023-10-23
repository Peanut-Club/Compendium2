using System.IO;

namespace Compendium.Packaging
{
    public interface IPackageWriter
    {
        void Write(BinaryWriter writer, IPackage data);
    }
}