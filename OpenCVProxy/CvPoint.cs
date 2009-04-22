using System;
using System.Runtime.InteropServices;

namespace OpenCVProxy
{
    /// <summary>
    /// 2D point with integer coordinates
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CvPoint
    {
        /// <summary>
        /// x-coordinate, usually zero-based 
        /// </summary>
        public int x;
        /// <summary>
        /// y-coordinate, usually zero-based 
        /// </summary>
        public int y;

        /// <summary>
        /// the constructor function
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        public CvPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static int Distance2(CvPoint p1, CvPoint p2)
        {
            int dx = p1.x - p2.x;
            int dy = p1.y - p2.y;
            return dx * dx + dy * dy;
        }
    }

    /// <summary>
    /// pixel-accurate size of a rectangle
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CvSize
    {
        /// <summary>
        /// width of the rectangle 
        /// </summary>
        public int width;

        /// <summary>
        /// height of the rectangle 
        /// </summary>
        public int height;

        /// <summary>
        /// the constructor function 
        /// </summary>
        /// <param name="width">width of the rectangle</param>
        /// <param name="height">height of the rectangle</param>
        public CvSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    /// <summary>
    /// offset and size of a rectangle
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CvRect
    {
        /// <summary>
        /// x-coordinate of the left-most rectangle corner[s]
        /// </summary>
        public int x;

        /// <summary>
        /// y-coordinate of the top-most or bottom-most rectangle corner[s]
        /// </summary>
        public int y;

        /// <summary>
        /// width of the rectangle 
        /// </summary>
        public int width;

        /// <summary>
        /// height of the rectangle 
        /// </summary>
        public int height;

        public CvPoint Offset
        {
            get { return new CvPoint(x, y); }
        }

        public CvSize Size
        {
            get { return new CvSize(width, height); }
        }

        public CvRect(CvPoint offset, CvSize size)
        {
            this.x = offset.x;
            this.y = offset.y;
            this.width = size.width;
            this.height = size.height;
        }

        public CvRect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}
