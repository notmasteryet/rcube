using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCVProxy.Interop
{
    public sealed class Int32SafeMemoryBox : SafeMemoryBox<Int32>
    {
        public Int32SafeMemoryBox()
            : base()
        {
        }

        public Int32SafeMemoryBox(int count)
            : base(count)
        {
        }

        public Int32SafeMemoryBox(Int32[] items)
            : base(items)
        {
        }


        public override int GetValue(int index)
        {
            return Marshal.ReadInt32(Pointer, index * Int32SafeMemoryBox.TypeSize);
        }

        public override void SetValue(int index, int value)
        {
            Marshal.WriteInt32(Pointer, index * Int32SafeMemoryBox.TypeSize, value);
        }
    }
}
