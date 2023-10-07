using System;

namespace Compendium.Pooling.Exceptions
{
    public class ItemAlreadyReturnedException : Exception
    {
        public ItemAlreadyReturnedException() : base("Attempted to add an item that was already in the pool's buffer.") { }
    }
}