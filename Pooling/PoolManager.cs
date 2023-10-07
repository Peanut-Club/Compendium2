using Compendium.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Compendium.Pooling
{
    public static class PoolManager
    {
        private static readonly List<GenericPoolBase> _genericPools = new List<GenericPoolBase>();
        private static readonly PoolBase _poolableInstance = new PoolBase(new PoolBuffer(RandomGeneration.String(5)));

        public static PoolBase Poolables => _poolableInstance;

        public static GenericPool<TItem> GetPool<TItem>() where TItem : new()
        {
            foreach (var pool in _genericPools)
            {
                if (pool.PoolType == typeof(TItem) && pool is GenericPool<TItem> genericPool)
                    return genericPool;
            }

            throw new InvalidOperationException($"There are no registered pools for type '{typeof(TItem).FullName}'");
        }

        public static TPoolable RentPoolable<TPoolable>() where TPoolable : PoolableBase, new()
            => Poolables.Buffer.NextItem(p => p is TPoolable, () => new TPoolable()) as TPoolable;

        public static void ReturnPoolable(PoolableBase poolable)
            => Poolables.Return(poolable);

        public static TItem Rent<TItem>() where TItem : new()
        {
            foreach (var pool in _genericPools)
            {
                if (pool.PoolType == typeof(TItem) && pool is GenericPool<TItem> genericPool)
                    return genericPool.Rent();
            }

            throw new InvalidOperationException($"There are no registered pools for type '{typeof(TItem).FullName}'");
        }

        public static void Return<TItem>(TItem item) where TItem : new()
        {
            foreach (var pool in _genericPools)
            {
                if (pool.PoolType == typeof(TItem) && pool is GenericPool<TItem> genericPool)
                {
                    genericPool.Return(item);
                    return;
                }
            }

            throw new InvalidOperationException($"There are no registered pools for type '{typeof(TItem).FullName}'");
        }

        public static void AddPool(GenericPoolBase genericPoolBase)
        {
            if (genericPoolBase is null)
                throw new ArgumentNullException(nameof(genericPoolBase));

            if (genericPoolBase.PoolType is null)
                throw new InvalidOperationException($"Tried registering a pool of null pool type.");

            if (_genericPools.Contains(genericPoolBase))
                throw new InvalidOperationException($"Generic pool '{genericPoolBase}' is already registered.");

            if (_genericPools.Any(pool => pool.PoolType == genericPoolBase.PoolType))
                throw new InvalidOperationException($"There's already a pool for type '{genericPoolBase.PoolType.FullName}'");

            _genericPools.Add(genericPoolBase);
        }

        public static void RemovePool(GenericPoolBase genericPoolBase)
        {
            if (genericPoolBase is null)
                throw new ArgumentNullException(nameof(genericPoolBase));

            if (genericPoolBase.PoolType is null)
                throw new InvalidOperationException($"Tried unregistering a pool of null pool type.");

            if (!_genericPools.Contains(genericPoolBase))
                throw new InvalidOperationException($"Generic pool '{genericPoolBase}' is not registered.");

            _genericPools.Remove(genericPoolBase);
        }
    }
}