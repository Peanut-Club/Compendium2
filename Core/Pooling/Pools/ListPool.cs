using System;
using System.Collections.Generic;

namespace Compendium.Pooling.Pools
{
    public class ListPool<TElement> : GenericPool<List<TElement>>
    {
        public static ListPool<TElement> Shared { get; } = new ListPool<TElement>();

        public override void OnReturning(List<TElement> item)
        {
            base.OnReturning(item);

            item.Clear();
        }

        public List<TElement> Rent(int minCapacity)
        {
            if (minCapacity <= 0)
                throw new InvalidOperationException($"minCapacity cannot be less than 1.");

            var list = Rent();

            if (list.Capacity < minCapacity)
                list.Capacity = minCapacity;

            return list;
        }

        public List<TElement> Rent(IEnumerable<TElement> elements)
        {
            if (elements is null)
                throw new ArgumentNullException(nameof(elements));

            var list = Rent();

            list.AddRange(elements);

            return list;
        }

        public TElement[] ToArrayReturn(List<TElement> elements)
        {
            if (elements is null)
                throw new ArgumentNullException(nameof(elements));

            var array = elements.ToArray();

            Return(elements);

            return array;
        }
    }
}