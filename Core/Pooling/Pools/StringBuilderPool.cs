using System.Text;
using System;

namespace Compendium.Pooling.Pools
{
    public class StringBuilderPool : GenericPool<StringBuilder>
    {
        public static StringBuilderPool Shared { get; } = new StringBuilderPool();

        public override void OnReturning(StringBuilder item)
        {
            base.OnReturning(item);

            item.Clear();
        }

        public StringBuilder Rent(int minCapacity)
        {
            if (minCapacity <= 0)
                throw new InvalidOperationException($"minCapacity cannot be less than 1.");

            var builder = Rent();

            if (builder.Capacity < minCapacity)
                builder.Capacity = minCapacity;

            return builder;
        }

        public StringBuilder Rent(params string[] initialLines)
        {
            var builder = Rent();

            for (int i = 0; i < initialLines.Length; i++)
                builder.AppendLine(initialLines[i]);

            return builder;
        }

        public string StringReturn(StringBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var str = builder.ToString();

            Return(builder);

            return str;
        }

        public string StringReturn(StringBuilder builder, int index, int length)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var str = builder.ToString(index, length);

            Return(builder);

            return str;
        }
    }
}