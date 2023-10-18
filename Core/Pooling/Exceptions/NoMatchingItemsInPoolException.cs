using System;

namespace Compendium.Pooling.Exceptions
{
    public class NoMatchingItemsInPoolException : Exception
    {
        public NoMatchingItemsInPoolException() : base("No items matching the provided predicate were found in the pool buffer.") { }
    }
}