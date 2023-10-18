using Compendium.Packaging;

using System.IO;

namespace Compendium.Assemblies.Packaging
{
    public class AssemblyPackageReader : PackageReader
    {
        public override PackageData Read(BinaryReader reader)
        {
            var data = new AssemblyPackageData();

            data.Read(reader);

            return data;
        }
    }
}