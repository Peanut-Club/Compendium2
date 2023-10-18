using System;

namespace Compendium.Pooling.Exceptions
{
    public class EmptyPoolException : Exception
    {
        public EmptyPoolException(string id) : base($"Pool '{id}' is empty and failed to create any new objects.") { }
    }
}