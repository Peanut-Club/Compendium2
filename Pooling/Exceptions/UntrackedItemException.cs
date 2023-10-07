using System;

namespace Compendium.Pooling.Exceptions
{
    public class UntrackedItemException : Exception
    {
        public UntrackedItemException() : base("Attempted to add an item of unknown origin to the pool buffer.") { }
    }
}