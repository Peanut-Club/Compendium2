using Compendium.Utilities.Reflection;

using System;
using System.IO;

namespace Compendium.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static TElement[] ReadArray<TElement>(this BinaryReader reader, Func<BinaryReader, TElement> readerStep)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            if (readerStep is null)
                throw new ArgumentNullException(nameof(readerStep));

            var size = reader.ReadInt32();

            if (size < 0)
                throw new InvalidDataException($"Cannot read {typeof(TElement).FullName} array; read size is invalid.");

            var array = new TElement[size];

            for (int i = 0; i < size; i++)
                array[i] = readerStep.SafeCall(reader);

            return array;
        }
    }
}