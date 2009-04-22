using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using OpenCVProxy.Interop;

namespace OpenCVProxy
{
    public class CvMemoryStorage : CvUnmanagedObject 
    {
        public CvMemoryStorage()
            : this(0)
        {
        }

        public CvMemoryStorage(int blockSize)
            : base(CxCore.cvCreateMemStorage(blockSize))
        {
        }

        public void Clear()
        {
            CxCore.cvClearMemStorage(Handle);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            using(IntPtrSafeMemoryBox box = new IntPtrSafeMemoryBox())
            {
                box.Value = Handle;
                CxCore.cvReleaseMemStorage(box.Pointer);
            }
        }
    }
}
