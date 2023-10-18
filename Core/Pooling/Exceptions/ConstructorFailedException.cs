using System;

namespace Compendium.Pooling.Exceptions
{
    public class ConstructorFailedException : Exception
    {
        public ConstructorFailedException(Type type) : base($"The provided constructor failed to provide an item of type '{type.FullName}'") { }
    }
}