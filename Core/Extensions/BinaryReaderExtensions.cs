using Compendium.Utilities.Reflection;

using System;
using System.IO;

namespace Compendium.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static void ReadSegments<TSegment>(        
            this BinaryReader reader, 
            
            Func<BinaryReader, TSegment> readerStep, 
            
            ref TSegment seg1, 
            ref TSegment seg2, 
            ref TSegment seg3, 
            ref TSegment seg4)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            if (readerStep is null)
                throw new ArgumentNullException(nameof(readerStep));

            var size = reader.ReadInt32();

            if (size < 4)
                throw new InvalidDataException($"Cannot read 4 segments of {typeof(TSegment).FullName}; read size is invalid.");

            seg1 = readerStep.SafeCall(reader);
            seg2 = readerStep.SafeCall(reader);
            seg3 = readerStep.SafeCall(reader);
            seg4 = readerStep.SafeCall(reader);
        }

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