using System;

namespace Compendium.Utilities
{
    public class Disposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public bool IsDisposing { get; private set; }

        public virtual void DisposeInternal() { }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposing = true;

                DisposeInternal();

                IsDisposing = false;
                IsDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}