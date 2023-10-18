using Compendium.Pooling.Exceptions;

using System;

namespace Compendium.Pooling
{
    public class PoolableBase
    {
        public PoolBase Pool { get; private set; }

        public bool IsPooled { get; private set; }

        public virtual void OnPooled() { }
        public virtual void OnUnpooled() { }

        public void ReturnToPool()
        {
            if (Pool is null)
                throw new InvalidOperationException("Tried to return a poolable that does not originate from a pool.");

            Pool.Return(this);
        }

        internal void ToPool()
        {
            IsPooled = true;
            OnPooled();
        }

        internal void UnPool()
        {
            IsPooled = false;
            OnUnpooled();
        }

        internal void SetPool(PoolBase pool)
        {
            if (Pool != null && Pool != pool)
                throw new MismatchedPoolException();

            Pool = pool;
        }
    }
}
