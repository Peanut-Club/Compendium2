using System;

namespace Compendium.Utilities
{
    public class Disposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public virtual void DisposeInternal() { }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                DisposeInternal();
                IsDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}