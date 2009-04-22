using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCVProxy.Interop
{
    public static class CvTypeSizes
    {
        public const int CvSeqSize = 14 * 4;
        public const int CvContourSize = CvSeqSize + 8 * 4;
    }
}
