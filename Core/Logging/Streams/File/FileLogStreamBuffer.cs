using Compendium.Pooling.Pools;
using Compendium.Utilities;

using System;
using System.Collections.Generic;
using System.Threading;

namespace Compendium.Logging.Streams.File
{
    public class FileLogStreamBuffer : Disposable
    {
        private List<string> _buffer;
        private Timer _timer;

        public int Size => _buffer.Count;

        public bool IsEmpty => _buffer.Count <= 0;
        public bool IsFlushing { get; private set; }

        public int Interval
        {
            set => _timer.Change(0, value);
        }

        public event Action<string[]> OnFlushing;

        public FileLogStreamBuffer(int interval)
        {
            _buffer = ListPool<string>.Shared.Rent();

            _timer = new Timer(_ =>
            {
                if (_buffer is null || IsEmpty)
                    return;

                IsFlushing = true;

                OnFlushing(ListPool<string>.Shared.ToArrayReturn(_buffer));

                _buffer = ListPool<string>.Shared.Rent();

                IsFlushing = false;
            }, null, 0, interval);
        }

        public void Append(string line)
        {
            if (IsFlushing)
                return;

            _buffer?.Add(line);
        }

        public override void DisposeInternal()
        {
            ListPool<string>.Shared.Return(_buffer);

            _buffer = null;

            base.DisposeInternal();
        }
    }
}