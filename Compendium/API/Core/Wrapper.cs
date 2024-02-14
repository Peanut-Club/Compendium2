using Common.Utilities;
using Common.Values;

namespace Compendium.API.Core
{
    public class Wrapper<T> : Disposable, IWrapper<T>
    {
        public Wrapper(T baseValue)
            => Base = baseValue;

        public T Base { get; }
    }
}