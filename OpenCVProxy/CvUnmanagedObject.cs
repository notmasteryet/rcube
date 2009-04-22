using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCVProxy
{
    public class CvUnmanagedObject : IDisposable
    {
        IntPtr handle;
        private bool disposed;

        public IntPtr Handle
        {
            get { return handle; }
        }

        protected CvUnmanagedObject()
        {
            this.handle = IntPtr.Zero;
            this.disposed = false;
        }

        protected CvUnmanagedObject(IntPtr handle)
        {
            this.handle = handle;
            this.disposed = false;
        }

        ~CvUnmanagedObject()
        {
            Dispose(false);
        }

        protected void SetHandle(IntPtr handle)
        {
            this.handle = handle;
        }

        public void Dispose()
        {
            if (disposed) return;

            GC.SuppressFinalize(this);

            Dispose(true);
            disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public static implicit operator IntPtr(CvUnmanagedObject obj)
        {
            return obj.Handle;
        }
    }
}
