using Compendium.Extensions;
using Compendium.Packaging;

using System.IO;

namespace Compendium.Assemblies.Packaging
{
    public class AssemblyPackageData : PackageData
    {
        public int VersionMajor;
        public int VersionMinor;
        public int VersionPatch;

        public string Name;
        public string Author;
        public string Description;
        public string Hash;

        public byte[] RawAssembly;
        public byte[][] RawDependencies;

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);

            VersionMajor = reader.ReadInt32();
            VersionMinor = reader.ReadInt32();
            VersionPatch = reader.ReadInt32();

            Name = reader.ReadString();
            Author = reader.ReadString();
            Description = reader.ReadString();
            Hash = reader.ReadString();

            RawAssembly = reader.ReadArray(r => r.ReadByte());
            RawDependencies = reader.ReadArray(r => r.ReadArray(re => re.ReadByte()));
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(VersionMajor);
            writer.Write(VersionMinor);
            writer.Write(VersionPatch);

            writer.Write(Name);
            writer.Write(Author);
            writer.Write(Description);
            writer.Write(Hash);

            writer.WriteArray(RawAssembly, (w, b) => w.Write(b));
            writer.WriteArray(RawDependencies, (w, b) => w.WriteArray(b, (w2, b2) => w2.Write(b2)));
        }
    }
}