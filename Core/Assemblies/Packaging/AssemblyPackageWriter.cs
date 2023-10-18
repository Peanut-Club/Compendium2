using Compendium.Packaging;

using System.IO;

namespace Compendium.Assemblies.Packaging
{
    public class AssemblyPackageWriter : PackageWriter
    {
        public override void Write(BinaryWriter writer, PackageData data)
        {
            if (data is AssemblyPackageData assemblyPackage)
                assemblyPackage.Write(writer);
        }
    }
}