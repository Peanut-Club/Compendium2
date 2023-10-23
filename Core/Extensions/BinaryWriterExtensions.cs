using System.IO;
using System;

using Compendium.Utilities.Reflection;

namespace Compendium.Extensions
{
    public static class BinaryWriterExtensions
    {
        public static void WriteSegments<TSegment>(this BinaryWriter writer, Action<BinaryWriter, TSegment> writerStep, params TSegment[] segments)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            if (writerStep is null)
                throw new ArgumentNullException(nameof(writerStep));

            writer.Write(segments.Length);

            for (int i = 0; i < segments.Length; i++)
                writerStep.SafeCall(writer, segments[i]);
        }

        public static void WriteArray<TElement>(this BinaryWriter writer, TElement[] elements, Action<BinaryWriter, TElement> writerStep)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            if (elements is null)
                throw new ArgumentNullException(nameof(elements));

            if (writerStep is null)
                throw new ArgumentNullException(nameof(writerStep));

            writer.Write(elements.Length);

            if (elements.Length > 0)
            {
                for (int i = 0; i < elements.Length; i++)
                    writerStep.SafeCall(writer, elements[i]);
            }
        }
    }
}