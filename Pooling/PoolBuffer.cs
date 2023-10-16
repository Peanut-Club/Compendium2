using System;
using System.Collections.Generic;

using Compendium.Pooling.Exceptions;
using Compendium.Utilities.Reflection;

namespace Compendium.Pooling
{
    public class PoolBuffer
    {
        private readonly HashSet<PoolableBase> _buffer = new HashSet<PoolableBase>();
        private readonly HashSet<PoolableBase> _tracked = new HashSet<PoolableBase>();

        public virtual int Size => _buffer.Count;

        public virtual bool IsEmpty => _buffer.Count <= 0;

        public virtual bool IsTrackingEnabled { get; set; } = true;

        public virtual string Id { get; }

        public PoolBuffer(string id)
        {
            Id = id;
        }

        public virtual void ReturnItem(PoolableBase poolable)
        {
            if (poolable is null)
                throw new ArgumentNullException(nameof(poolable));

            if (IsTrackingEnabled && !_tracked.Contains(poolable))
                throw new UntrackedItemException();

            if (_buffer.Contains(poolable))
                throw new ItemAlreadyReturnedException();

            _buffer.Add(poolable);
        }

        public virtual PoolableBase NextItem(Predicate<PoolableBase> predicate = null, Func<PoolableBase> constructor = null)
        {
            if (IsEmpty)
            {
                if (constructor is null)
                    throw new EmptyPoolException(Id);

                var item = constructor.SafeCall();

                if (item is null)
                    throw new ConstructorFailedException(constructor.GetGenericType());

                return constructor.SafeCall();
            }

            foreach (var item in _buffer)
            {
                if (predicate != null && predicate.SafeCall(item))
                    return item;

                if (predicate is null)
                    return item;
            }

            throw new NoMatchingItemsInPoolException();
        }

        public void Clear()
        {
            _buffer.Clear();
            _tracked.Clear();
        }
    }
}