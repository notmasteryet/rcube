using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCVProxy.Interop
{
    public sealed class StructureSafeMemoryBox<T> : SafeMemoryBox<T>
    {
        public StructureSafeMemoryBox()
            : base()
        {
        }

        public StructureSafeMemoryBox(int count)
            : base(count)
        {
        }

        public StructureSafeMemoryBox(T[] items)
            : base(items)
        {
        }

        public override T GetValue(int index)
        {
            IntPtr offset = new IntPtr(Pointer.ToInt32() + index * StructureSafeMemoryBox<T>.TypeSize);
            return (T)Marshal.PtrToStructure(offset, typeof(T));
        }

        public override void SetValue(int index, T value)
        {
            IntPtr offset = new IntPtr(Pointer.ToInt32() + index * StructureSafeMemoryBox<T>.TypeSize);
            Marshal.StructureToPtr(value, offset, false);
        }
    }
}
