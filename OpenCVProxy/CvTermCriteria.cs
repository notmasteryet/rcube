using System;
using System.Runtime.InteropServices;

namespace OpenCVProxy
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CvTermCriteria
    {
        public int type;  /* a combination of CV_TERMCRIT_ITER and CV_TERMCRIT_EPS */
        public int max_iter; /* maximum number of iterations */
        public double epsilon; /* accuracy to achieve */

        public CvTermCriteria(int max_iter)
        {
            this.type = 1;
            this.max_iter = max_iter;
            this.epsilon = default(double);
        }

        public CvTermCriteria(double epsilon)
        {
            this.type = 2;
            this.max_iter = default(int);
            this.epsilon = epsilon;
        }

        public CvTermCriteria(int max_iter, double epsilon)
        {
            this.type = 3;
            this.max_iter = max_iter;
            this.epsilon = epsilon;
        }

        public CvTermCriteria(int type, int max_iter, double epsilon)
        {
            this.type = type;
            this.max_iter = max_iter;
            this.epsilon = epsilon;
        }
    }
}
