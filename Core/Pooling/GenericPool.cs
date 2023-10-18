using Compendium.Utilities;

using System;

namespace Compendium.Pooling
{
    public class GenericPool<TItem> : GenericPoolBase where TItem : new()
    {
        public GenericPoolBuffer<TItem> Buffer { get; } = new GenericPoolBuffer<TItem>(RandomGeneration.String(5));

        public override Type PoolType { get; } = typeof(TItem);

        public GenericPool()
            => PoolManager.AddPool(this);

        ~GenericPool()
            => PoolManager.RemovePool(this);

        public virtual void OnRenting(TItem item) { }
        public virtual void OnReturning(TItem item) { }

        public TItem Rent()
        {
            var item = Buffer.NextItem();

            OnRenting(item);

            return item;
        }

        public void Return(TItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            OnReturning(item);

            Buffer.ReturnItem(item);
        }
    }
}