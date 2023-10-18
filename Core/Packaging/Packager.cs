using System;
using System.Collections.Generic;
using System.IO;

namespace Compendium.Packaging
{
    public static class Packager
    {
        private static readonly Dictionary<Type, PackageReader> _readers = new Dictionary<Type, PackageReader>();
        private static readonly Dictionary<Type, PackageWriter> _writers = new Dictionary<Type, PackageWriter>();

        public static void SetReaderWriter<TPackageData, TPackageWriter, TPackageReader>()
            where TPackageData : PackageData, new()
            where TPackageReader : PackageReader, new()
            where TPackageWriter : PackageWriter, new()
        {
            SetReader<TPackageData, TPackageReader>();
            SetWriter<TPackageData, TPackageWriter>();
        }

        public static void SetWriter<TPackageData, TPackageWriter>()
            where TPackageData : PackageData, new()
            where TPackageWriter : PackageWriter, new()
        {
            if (_writers.ContainsKey(typeof(TPackageData)))
                throw new InvalidOperationException($"There already is a writer for type '{typeof(TPackageData).FullName}'");

            _writers[typeof(TPackageData)] = new TPackageWriter();
        }

        public static void SetReader<TPackageData, TPackageReader>()
            where TPackageData : PackageData, new()
            where TPackageReader : PackageReader, new()
        {
            if (_readers.ContainsKey(typeof(TPackageData)))
                throw new InvalidOperationException($"There already is a reader for type '{typeof(TPackageData).FullName}'");

            _readers[typeof(TPackageData)] = new TPackageReader();
        }

        public static PackageReader GetReader<TPackageData>() where TPackageData : PackageData, new()
            => _readers.TryGetValue(typeof(TPackageData), out var reader) ? reader : throw new NotImplementedException($"There are no readers registered for type '{typeof(TPackageData).FullName}'");

        public static PackageWriter GetWriter<TPackageData>() where TPackageData : PackageData, new()
            => _writers.TryGetValue(typeof(TPackageData), out var writer) ? writer : throw new NotImplementedException($"There are no writers registered for type '{typeof(TPackageData).FullName}'");

        public static TPackageData ReadPackage<TPackageData>(byte[] packageBytes) where TPackageData : PackageData, new()
        {
            if (packageBytes is null)
                throw new ArgumentNullException(nameof(packageBytes));

            if (packageBytes.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(packageBytes));

            var packageReader = GetReader<TPackageData>();

            using (var memStream = new MemoryStream(packageBytes))
            using (var reader = new BinaryReader(memStream))
            {
                var package = packageReader.Read(reader);

                if (package is null)
                    throw new Exception($"Reader failed to read package");

                if (package is not TPackageData packageData)
                    throw new Exception($"Reader returned a wrong value");

                return packageData;
            }
        }

        public static byte[] WritePackage<TPackageData>(TPackageData packageData) where TPackageData : PackageData, new()
        {
            if (packageData is null)
                throw new ArgumentNullException(nameof(packageData));

            var packageWriter = GetWriter<TPackageData>();

            using (var memStream = new MemoryStream())
            using (var writer = new BinaryWriter(memStream))
            {
                packageWriter.Write(writer, packageData);

                return memStream.ToArray();
            }
        }
    }
}