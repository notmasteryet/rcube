using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCVProxy
{
    public class CvSeqNavigator : ICloneable
    {
        IntPtr pointer;

        public IntPtr Pointer
        {
            get { return pointer; }
        }

        public bool IsEmpty
        {
            get { return Pointer == IntPtr.Zero; }
        }

        public CvSeqNavigator(IntPtr pointer)
        {
            this.pointer = pointer;
        }

        private bool MoveTo(IntPtr pointer)
        {
            this.pointer = pointer;
            return pointer == IntPtr.Zero;
        }

        const int HPrevOffset = 2 * 4;
        const int HNextOffset = 3 * 4;
        const int VPrevOffset = 4 * 4;
        const int VNextOffset = 5 * 4;

        public bool Next()
        {
            return NextHorizontal();
        }

        public bool Previous()
        {
            return PreviousHorizontal();
        }

        public bool NextHorizontal()
        {
            return MoveTo(Marshal.ReadIntPtr(Pointer, HNextOffset));
        }

        public bool PreviousHorizontal()
        {
            return MoveTo(Marshal.ReadIntPtr(Pointer, HPrevOffset));
        }

        public bool NextVertical()
        {
            return MoveTo(Marshal.ReadIntPtr(Pointer, VNextOffset));
        }

        public bool PreviousVertical()
        {
            return MoveTo(Marshal.ReadIntPtr(Pointer, VPrevOffset));
        }

        public object Clone()
        {
            return (CvSeqNavigator)MemberwiseClone();
        }

        public static implicit operator IntPtr(CvSeqNavigator obj)
        {
            return obj.Pointer;
        }
    }
}
