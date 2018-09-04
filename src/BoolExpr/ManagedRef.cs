using System;

namespace BoolExprNet
{

    /// <summary>
    /// Base class for instances that hold onto an unmanaged reference.
    /// </summary>
    public abstract class ManagedRef :
        IDisposable
    {

        IntPtr ptr;
        bool disposed;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        protected ManagedRef(IntPtr ptr)
        {
            this.ptr = ptr;
        }

        /// <summary>
        /// Gets the native pointer.
        /// </summary>
        protected internal IntPtr Ptr => ptr;

        /// <summary>
        /// Override this method to invoke the associated C method to free the resource.
        /// </summary>
        /// <param name="ptr"></param>
        protected abstract void Free(IntPtr ptr);

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if (ptr != IntPtr.Zero)
                {
                    Free(ptr);
                    ptr = IntPtr.Zero;
                }

                disposed = true;
            }
        }

        ~ManagedRef()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

}