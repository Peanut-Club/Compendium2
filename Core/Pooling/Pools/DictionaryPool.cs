using System;
using System.Collections.Generic;
using System.Linq;

namespace Compendium.Pooling.Pools
{
    public class DictionaryPool<TKey, TElement> : GenericPool<Dictionary<TKey, TElement>>
    {
        public static DictionaryPool<TKey, TElement> Shared { get; } = new DictionaryPool<TKey, TElement>();

        public override void OnReturning(Dictionary<TKey, TElement> item)
        {
            base.OnReturning(item);
            item.Clear();
        }

        public Dictionary<TKey, TElement> Rent(IEnumerable<KeyValuePair<TKey, TElement>> elements)
        {
            if (elements is null)
                throw new ArgumentNullException(nameof(elements));

            var dict = Rent();

            foreach (var pair in elements)
                dict[pair.Key] = pair.Value;

            return dict;
        }

        public KeyValuePair<TKey, TElement>[] ToArrayReturn(Dictionary<TKey, TElement> elements)
        {
            if (elements is null)
                throw new ArgumentNullException(nameof(elements));

            var array = elements.ToArray();

            Return(elements);

            return array;
        }
    }
}
