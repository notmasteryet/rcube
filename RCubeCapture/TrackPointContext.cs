using System;
using System.Collections.Generic;
using System.Text;

using OpenCVProxy;
using OpenCVProxy.Interop;

namespace RCubeCapture
{
    class TrackPointContext : IDisposable
    {
        StructureSafeMemoryBox<CvPoint2D32f> points;

        public StructureSafeMemoryBox<CvPoint2D32f> Points
        {
            get { return points; }
        }

        int count;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        IplImage gray;

        public IplImage Gray
        {
            get { return gray; }
        }

        IplImage pyramid;

        public IplImage Pyramid
        {
            get { return pyramid; }
        }

        bool[] isFading;

        public bool[] IsFading
        {
            get { return isFading; } 
        }

        public TrackPointContext(CvSize imageSize, int maxPointsCount)
        {
            gray = new IplImage(imageSize, 8, 1);
            pyramid = new IplImage(imageSize, 8, 1);

            points = new StructureSafeMemoryBox<CvPoint2D32f>(maxPointsCount);
            isFading = new bool[maxPointsCount];
        }

        public void Dispose()
        {
            gray.Dispose();
            pyramid.Dispose();

            points.Dispose();
        }
    }
}
