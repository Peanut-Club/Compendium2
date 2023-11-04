namespace Compendium.Values
{
    /// <summary>
    /// Represents a wrapped class.
    /// </summary>
    /// <typeparam name="TValue">The type used with this wrapper.</typeparam>
    public interface IWrapper<TValue>
    {
        /// <summary>
        /// Gets the original instance of the class represented by this wrapper.
        /// </summary>
        public TValue Base { get; }
    }
}