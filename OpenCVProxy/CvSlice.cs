using System;
using System.Runtime.InteropServices;

namespace OpenCVProxy
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CvSlice
    {
        const int CV_WHOLE_SEQ_END_INDEX = 0x3fffffff;

        public static readonly CvSlice WholeSeq = new CvSlice(0, CV_WHOLE_SEQ_END_INDEX);

        public int start_index;
        public int end_index;

        public CvSlice(int start_index, int end_index)
        {
            this.start_index = start_index;
            this.end_index = end_index;
        }
    }
}
