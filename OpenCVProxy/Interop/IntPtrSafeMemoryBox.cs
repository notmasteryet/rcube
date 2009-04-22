using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCVProxy.Interop
{
    public sealed class IntPtrSafeMemoryBox : SafeMemoryBox<IntPtr>
    {
        public IntPtrSafeMemoryBox()
            : base()
        {
        }

        public IntPtrSafeMemoryBox(int count)
            : base(count)
        {
        }

        public IntPtrSafeMemoryBox(IntPtr item)
            : base(1)
        {
            Value = item;
        }

        public IntPtrSafeMemoryBox(IntPtr[] items)
            : base(items)
        {
        }

        public override IntPtr GetValue(int index)
        {
            return Marshal.ReadIntPtr(Pointer, index * Int32SafeMemoryBox.TypeSize);
        }

        public override void SetValue(int index, IntPtr value)
        {
            Marshal.WriteIntPtr(Pointer, index * Int32SafeMemoryBox.TypeSize, value);
        }
    }
}
