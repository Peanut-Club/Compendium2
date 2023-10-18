using System.IO;

namespace Compendium.Packaging
{
    public class PackageData
    {
        public string Id { get; set; }

        public PackageData() { }

        public virtual void Read(BinaryReader reader)
            => Id = reader.ReadString();

        public virtual void Write(BinaryWriter writer)
            => writer.Write(Id);
    }
}