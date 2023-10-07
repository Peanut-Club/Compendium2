using System;

namespace Compendium.Pooling.Exceptions
{
    public class MismatchedPoolException : Exception
    {
        public MismatchedPoolException() : base("This object cannot be placed in this pool because it has already been placed in another pool.") { }
    }
}