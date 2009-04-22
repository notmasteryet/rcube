using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCVProxy.Interop
{
    public static partial class Cv
    {
        public static double cvContourPerimeter(IntPtr contour)
        {
            return cvArcLength(contour, CvSlice.WholeSeq, 1);
        }
    }
}
