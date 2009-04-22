using System;
using System.Runtime.InteropServices;

namespace OpenCVProxy
{
    /// <summary>
    /// A container for 1-,2-,3- or 4-tuples of numbers
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CvScalar
    {
        public double item1;
        public double item2;
        public double item3;
        public double item4;

        public double R
        {
            get { return item3; }
        }

        public double G
        {
            get { return item2; }
        }

        public double B
        {
            get { return item1; }
        }

        public CvScalar(double item1)
        {
            this.item1 = item1;
            this.item2 = 0;
            this.item3 = 0;
            this.item4 = 0;
        }
        public CvScalar(double item1, double item2)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = 0;
            this.item4 = 0;
        }
        public CvScalar(double item1, double item2, double item3)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
            this.item4 = 0;
        }

        public static CvScalar FromRGB(double r, double g, double b)
        {
            return new CvScalar(b, g, r);
        }
    }
}
