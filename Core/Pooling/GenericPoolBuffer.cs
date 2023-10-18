using Compendium.Pooling.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Compendium.Pooling
{
    public class GenericPoolBuffer<TItem> where TItem : new()
    {
        private readonly HashSet<TItem> _buffer = new HashSet<TItem>();
        private readonly HashSet<TItem> _tracked = new HashSet<TItem>();

        public virtual int Size => _buffer.Count;

        public virtual bool IsEmpty => _buffer.Count <= 0;

        public virtual bool IsTrackingEnabled { get; set; } = true;

        public virtual string Id { get; }

        public event Action<TItem> OnItemPooled;
        public event Action<TItem> OnItemUnPooled;

        public GenericPoolBuffer(string id)
        {
            Id = id;
        }

        public virtual void ReturnItem(TItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (IsTrackingEnabled && !_tracked.Contains(item))
                throw new UntrackedItemException();

            if (_buffer.Contains(item))
                throw new ItemAlreadyReturnedException();

            _buffer.Add(item);

            OnItemPooled?.Invoke(item);
        }

        public virtual TItem NextItem()
        {
            if (IsEmpty)
            {
                var item = Activator.CreateInstance<TItem>();

                if (item is null)
                    throw new ConstructorFailedException(typeof(TItem));

                return item;
            }

            return _buffer.First();
        }

        public void Clear()
        {
            _buffer.Clear();
            _tracked.Clear();
        }
    }
}