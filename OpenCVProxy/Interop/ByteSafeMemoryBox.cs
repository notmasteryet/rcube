using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCVProxy.Interop
{
    public sealed class ByteSafeMemoryBox : SafeMemoryBox<byte>
    {
        public ByteSafeMemoryBox(int count)
            : base(count)
        {
        }

        public ByteSafeMemoryBox(byte[] data)
            : base(data)
        {
        }

        public override byte GetValue(int index)
        {
            return Marshal.ReadByte(Pointer, index);
        }

        public override void SetValue(int index, byte value)
        {
            Marshal.WriteByte(Pointer, index, value);
        }
    }
}
