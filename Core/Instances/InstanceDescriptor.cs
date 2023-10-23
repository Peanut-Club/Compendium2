using Compendium.Values;

using System;

namespace Compendium.Instances
{
    public class InstanceDescriptor
    {
        public WeakReferenceValue<object> Reference { get; }

        public Type Type { get; }

        public int HashCode { get; }

        public InstanceDescriptor(object value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            Type = value.GetType();
            Reference = new WeakReferenceValue<object>(value);
           
            try
            {
                HashCode = value.GetHashCode();
            }
            catch { HashCode = -1; }
        }
    }
}