using System;
using System.Collections.Generic;
using System.Text;

using OpenCVProxy.Interop;

namespace OpenCVProxy
{
    public sealed class CvCapture : CvUnmanagedObject 
    {
        public const int AnyCamera = -1;

        public CvCapture(int cameraIndex)
        {            
            IntPtr handle = HighGui.cvCreateCameraCapture(0);
            if (handle == IntPtr.Zero)
                throw new Exception("Camera is absent");
            SetHandle(handle);
        }

        public CvCapture(string filename)
        {
            IntPtr handle = HighGui.cvCreateFileCapture(filename);
            if (handle == IntPtr.Zero)
                throw new Exception("Cannot open capture from file: " + filename);
            SetHandle(handle);
        }

        public IplImage GetNextFrame()
        {
            IntPtr frame = HighGui.cvQueryFrame(Handle);
            return new IplImage(frame, false);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            using (IntPtrSafeMemoryBox releasePad = new IntPtrSafeMemoryBox())
            {
                releasePad.Value = Handle;
                HighGui.cvReleaseCapture(releasePad.Pointer);
            }
        }
    }
}
