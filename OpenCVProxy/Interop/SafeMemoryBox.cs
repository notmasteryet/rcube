using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCVProxy.Interop
{
    public abstract class SafeMemoryBox<T> : IDisposable
    {
        protected static int TypeSize = Marshal.SizeOf(typeof(T));

        IntPtr pointer;
        int count;
        bool disposed = false;
        
        public T this[int index]
        {
            get { return GetValue(index); }
            set { SetValue(index, value); }
        }

        public T Value
        {
            get { return GetValue(0); }
            set { SetValue(0, value); }
        }

        public int Count
        {
            get { return count; }
        }

        public IntPtr Pointer
        {
            get { return pointer; }
        }

        public SafeMemoryBox()
            : this(1)
        {
        }

        public SafeMemoryBox(int count)
        {
            AllocateMemory(count);
        }

        public SafeMemoryBox(T[] values)
        {
            AllocateMemory(values.Length);
        }

        public abstract T GetValue(int index);

        public abstract void SetValue(int index, T value);

        private void AllocateMemory(int count)
        {
            this.count = count;
            this.pointer = Marshal.AllocCoTaskMem(count * TypeSize);
        }

        private void FreeMemory()
        {
            Marshal.FreeCoTaskMem(this.pointer);
        }

        private void Dispose(bool disposing)
        {
            if (disposed) return;

            disposed = true;
            FreeMemory();
            if (disposing)
                GC.SuppressFinalize(this);
        }
        
        public void Dispose()
        {
            Dispose(true);
        }

        ~SafeMemoryBox()
        {
            Dispose(false);
        }
    }
}
