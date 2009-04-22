using System;
using System.Runtime.InteropServices;

namespace OpenCVProxy
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CvPoint2D32f
    {
        public float x; /* x-coordinate, usually zero-based */
        public float y; /* y-coordinate, usually zero-based */

        public CvPoint2D32f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public CvPoint ToCvPoint()
        {
            return new CvPoint((int)x, (int)y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CvSize2D32f
    {
        public float width;
        public float height; 

        public CvSize2D32f(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        public CvSize ToCvSize()
        {
            return new CvSize((int)width, (int)height);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CvBox2D
    {
        public CvPoint2D32f center;
        public CvSize2D32f size;
        public float angle;
    }
}
