using Compendium.Packaging;
using Compendium.Extensions;

using System.IO;
using System;

namespace Compendium.Plugins.Packaging
{
    public class AssemblyPackager : IPackageReader, IPackageWriter
    {
        public IPackage Read(BinaryReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var package = new PackagedAssembly();

            package.Name = reader.ReadString();
            package.Version = reader.ReadString();
            package.Culture = reader.ReadString();
            package.Token = reader.ReadString();

            package.Data = reader.ReadArray(br => br.ReadByte());

            return package;
        }

        public void Write(BinaryWriter writer, IPackage data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            if (data is not PackagedAssembly packagedAssembly)
                throw new ArgumentException($"Package '{data.GetType().FullName}' is not supported by this packager.");

            writer.Write(packagedAssembly.Name);
            writer.Write(packagedAssembly.Version);
            writer.Write(packagedAssembly.Culture);
            writer.Write(packagedAssembly.Token);

            writer.WriteArray(packagedAssembly.Data, (bw, b) => bw.Write(b));
        }
    }
}