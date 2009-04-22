using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using OpenCVProxy.Interop;

namespace OpenCVProxy
{
    public abstract class CvMat<T> : CvUnmanagedObject
    {
        CvMatHeader header;
        int channels;

        protected IntPtr Data
        {
            get { return header.data; }
        }

        protected int RowStep
        {
            get { return header.step; }
        }

        public int Rows
        {
            get { return header.rows; }
        }

        public int Cols
        {
            get { return header.cols; }
        }

        public int Channels
        {
            get { return channels; }
        }

        public T this[int row, int col]
        {
            get { return GetItem(row, col, 0); }
            set { SetItem(row, col, 0, value); }
        }
       
        public T this[int row, int col, int channel]
        {
            get { return GetItem(row, col, channel); }
            set { SetItem(row, col, channel, value); }
        }

        public CvMat(int rows, int cols, int channels, int dataType)
        {
            if (rows <= 0) throw new ArgumentOutOfRangeException("rows");
            if (cols <= 0) throw new ArgumentOutOfRangeException("cols");
            if (channels <= 0 || channels >= 64) throw new ArgumentOutOfRangeException("channels");
            if (dataType <= 0 || dataType > 7) throw new ArgumentOutOfRangeException("dataType");

            SetHandle(CxCore.cvCreateMat(rows, cols, MakeType(dataType, channels)));
            header = (CvMatHeader)Marshal.PtrToStructure(Handle, typeof(CvMatHeader));

            this.channels = channels;
        }

        protected static int MakeType(int depth, int channels)
        {
            return depth | ((channels - 1) << 3);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            using(IntPtrSafeMemoryBox releasePad = new IntPtrSafeMemoryBox())
            {
                releasePad.Value = Handle;
                CxCore.cvReleaseMat(releasePad.Pointer);
            }
        }

        public abstract T GetItem(int row, int col, int channel);

        public abstract void SetItem(int row, int col, int channel, T value);

    }

#pragma warning disable 649
    [StructLayout(LayoutKind.Sequential)]
    struct CvMatHeader
    {
        public int type;
        public int step;
        public IntPtr refCount;
        public int hdr_refCount;

        public IntPtr data;
        public int rows;
        public int cols;
    }
#pragma warning restore 649

    public class CvMatInt32 : CvMat<Int32>
    {
        public CvMatInt32(int rows, int cols) : this(rows, cols, 1)
        {
        }

        public CvMatInt32(int rows, int cols, int channels) : base(rows, cols, channels, CxCore.CV_32S)
        {
        }

        public override int GetItem(int row, int col, int channel)
        {
            return Marshal.ReadInt32(Data, row * RowStep + (col * Channels + channel) * 4);
        }

        public override void SetItem(int row, int col, int channel, int value)
        {
            Marshal.WriteInt32(Data, row * RowStep + (col * Channels + channel) * 4, value); 
        }
    }

    public class CvMatSingle : CvMat<Single>
    {
        const int SingleSize = 4;

        public CvMatSingle(int rows, int cols) : this(rows, cols, 1)
        {
        }

        public CvMatSingle(int rows, int cols, int channels)
            : base(rows, cols, channels, CxCore.CV_32F)
        {
        }

        public override float GetItem(int row, int col, int channel)
        {
            byte[] buf = new byte[SingleSize];
            Marshal.Copy(new IntPtr(Data.ToInt32() + row * RowStep + (col * Channels + channel) * SingleSize), buf, 0, SingleSize);
            return BitConverter.ToSingle(buf, 0);
        }

        public override void SetItem(int row, int col, int channel, float value)
        {
            byte[] buf = BitConverter.GetBytes(value);
            Marshal.Copy(buf, 0, new IntPtr(Data.ToInt32() + row * RowStep + (col * Channels + channel) * SingleSize), SingleSize);
        }
    }

}
