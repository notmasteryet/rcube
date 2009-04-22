using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using OpenCVProxy.Interop;

namespace OpenCVProxy
{
    public static class CvUtils
    {
        public static void InitializeCv()
        {
            CxCore.InitializeLibrary();
            Cv.InitializeLibrary();
            HighGui.InitializeLibrary();
        }
    }
}
