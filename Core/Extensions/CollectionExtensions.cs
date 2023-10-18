using System;
using System.Collections.Generic;
using System.Linq;

namespace Compendium.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsMatch<TElement>(this IEnumerable<TElement> collection, IEnumerable<TElement> target)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (target is null)
                throw new ArgumentNullException(nameof(target));

            if (collection.Count() != target.Count())
                return false;

            for (int i = 0; i < collection.Count(); i++)
            {
                var element = collection.ElementAt(i);
                var matchElement = target.ElementAt(i);

                if (!element.Equals(matchElement))
                    return false;
            }

            return true;
        }
    }
}
