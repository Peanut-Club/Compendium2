using System;

namespace Compendium.Pooling
{
    public class PoolBase
    {
        public virtual PoolBuffer Buffer { get; }

        public virtual Predicate<PoolableBase> BufferPredicate { get; }
        public virtual Func<PoolableBase> BufferConstructor { get; }

        public PoolBase(PoolBuffer buffer)
            => Buffer = buffer;

        public virtual PoolableBase Rent()
        {
            if (Buffer is null)
                throw new InvalidOperationException($"Tried renting an item from an invalid pool (pool buffer is null).");

            var item = Buffer.NextItem(BufferPredicate, BufferConstructor);

            item.UnPool();

            return item;
        }

        public virtual void Return(PoolableBase poolable)
        {
            if (Buffer is null)
                throw new InvalidOperationException($"Tried returning an item on an invalid pool (pool buffer is null).");

            Buffer.ReturnItem(poolable);

            poolable.SetPool(this);
            poolable.ToPool();
        }
    }
}