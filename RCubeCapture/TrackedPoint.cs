using System;
using System.Collections.Generic;
using System.Text;

using OpenCVProxy;
using OpenCVProxy.Interop;

namespace RCubeCapture
{
    struct TrackedPoint
    {
        public CvPoint2D32f p;
        public CvPoint2D32f offset;
        public bool isNewPoint;
    }
}
