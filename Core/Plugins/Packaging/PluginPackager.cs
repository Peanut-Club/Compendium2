using Compendium.Extensions;
using Compendium.Packaging;

using System;
using System.IO;

namespace Compendium.Plugins.Packaging
{
    public class PluginPackager : IPackageReader, IPackageWriter
    {
        public IPackage Read(BinaryReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var packagedPlugin = new PackagedPlugin();

            packagedPlugin.Metadata = new PluginMetadata();
            packagedPlugin.Symbols = new PluginSymbols();

            packagedPlugin.Metadata.Version = new PluginVersion();
            packagedPlugin.Metadata.MinVersion = new PluginVersion();
            packagedPlugin.Metadata.MaxVersion = new PluginVersion();

            packagedPlugin.Metadata.Name = reader.ReadString();
            packagedPlugin.Metadata.Description = reader.ReadString();
            packagedPlugin.Metadata.Author = reader.ReadString();

            reader.ReadSegments(br => br.ReadInt32(),
                ref packagedPlugin.Metadata.Version.Major,
                ref packagedPlugin.Metadata.Version.Minor,
                ref packagedPlugin.Metadata.Version.Patch,
                ref packagedPlugin.Metadata.Version.Build);

            reader.ReadSegments(br => br.ReadInt32(),
                ref packagedPlugin.Metadata.MinVersion.Major,
                ref packagedPlugin.Metadata.MinVersion.Minor,
                ref packagedPlugin.Metadata.MinVersion.Patch,
                ref packagedPlugin.Metadata.MinVersion.Build);

            reader.ReadSegments(br => br.ReadInt32(),
                ref packagedPlugin.Metadata.MaxVersion.Major,
                ref packagedPlugin.Metadata.MaxVersion.Minor,
                ref packagedPlugin.Metadata.MaxVersion.Patch,
                ref packagedPlugin.Metadata.MaxVersion.Build);

            packagedPlugin.Symbols.IsDebug = reader.ReadBoolean();
            packagedPlugin.Symbols.IsTrace = reader.ReadBoolean();

            var assemblyBytes = reader.ReadArray(br => br.ReadByte());

            packagedPlugin.Assembly = Packager.ReadPackage<PackagedAssembly>(assemblyBytes);

            packagedPlugin.Dependencies = reader.ReadArray(br =>
            {
                var depBytes = reader.ReadArray(br => br.ReadByte());

                return Packager.ReadPackage<PackagedAssembly>(depBytes);
            });

            return packagedPlugin;
        }

        public void Write(BinaryWriter writer, IPackage data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            if (data is not PackagedPlugin packagedPlugin)
                throw new ArgumentException($"Package '{data.GetType().FullName}' is not supported by this packager.");

            writer.Write(packagedPlugin.Metadata.Name);
            writer.Write(packagedPlugin.Metadata.Description);
            writer.Write(packagedPlugin.Metadata.Author);

            writer.WriteSegments((bw, val) => bw.Write(val), 
                packagedPlugin.Metadata.Version.Major, 
                packagedPlugin.Metadata.Version.Minor, 
                packagedPlugin.Metadata.Version.Patch, 
                packagedPlugin.Metadata.Version.Build);

            writer.WriteSegments((bw, val) => bw.Write(val),
                packagedPlugin.Metadata.MinVersion.Major,
                packagedPlugin.Metadata.MinVersion.Minor,
                packagedPlugin.Metadata.MinVersion.Patch,
                packagedPlugin.Metadata.MinVersion.Build);

            writer.WriteSegments((bw, val) => bw.Write(val),
                packagedPlugin.Metadata.MaxVersion.Major,
                packagedPlugin.Metadata.MaxVersion.Minor,
                packagedPlugin.Metadata.MaxVersion.Patch,
                packagedPlugin.Metadata.MaxVersion.Build);

            writer.Write(packagedPlugin.Symbols.IsDebug);
            writer.Write(packagedPlugin.Symbols.IsTrace);

            var assemblyBytes = Packager.WritePackage(packagedPlugin.Assembly);

            writer.WriteArray(assemblyBytes, (bw, b) => bw.Write(b));

            writer.WriteArray(packagedPlugin.Dependencies, (bw, dep) =>
            {
                var depBytes = Packager.WritePackage(dep);

                writer.WriteArray(depBytes, (bw, b) => bw.Write(b));
            });
        }
    }
}