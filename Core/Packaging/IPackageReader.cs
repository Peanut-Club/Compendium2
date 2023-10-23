using System.IO;

namespace Compendium.Packaging
{
    public interface IPackageReader
    {
        IPackage Read(BinaryReader reader);
    }
}