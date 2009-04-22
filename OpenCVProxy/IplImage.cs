using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using OpenCVProxy.Interop;

namespace OpenCVProxy
{
    public sealed class IplImage : CvUnmanagedObject, ICloneable 
    {
        bool ownHandle;
        ImageInfo imageInfo;

        public CvSize Size
        {
            get { return new CvSize(imageInfo.width, imageInfo.height); }            
        }

        public int Depth
        {
            get { return imageInfo.depth; }
        }

        public int Channels
        {
            get { return imageInfo.nChannels; }
        }

        public int Origin
        {
            get { return imageInfo.origin; }
        }

        internal IplImage(IntPtr handle, bool ownHandle) : base(handle)
        {
            this.ownHandle = ownHandle;

            ReadImageInfo();
        }

        public IplImage(CvSize size, int depth, int channels) : base(CreateImage(size, depth, channels))
        {
            this.ownHandle = true;

            ReadImageInfo();
        }

        public IplImage(string filename, int isColor) : base(LoadImageFromFile(filename, isColor))
        {
            this.ownHandle = true;

            ReadImageInfo();
        }

        private void ReadImageInfo()
        {
            imageInfo = (ImageInfo)Marshal.PtrToStructure(Handle, typeof(ImageInfo)); 
        }

        private static IntPtr CreateImage(CvSize size, int depth, int channels)
        {
            return CxCore.cvCreateImage(size, depth, channels);
        }

        private static IntPtr LoadImageFromFile(string filename, int isColor)
        {
            IntPtr handle = HighGui.cvLoadImage(filename, isColor);
            if (handle == IntPtr.Zero)
                throw new Exception("Image not loaded");
            return handle;
        }

        public CvScalar GetPixel(int x, int y)
        {
            int pixelSize = imageInfo.nChannels * (imageInfo.depth / 8);
            int offset = pixelSize * x + imageInfo.widthStep * y;
            IntPtr address = new IntPtr(imageInfo.imageData.ToInt32() + offset);

            return new CvScalar(Marshal.ReadByte(address, 0), Marshal.ReadByte(address, 1), Marshal.ReadByte(address, 2));
        }

        public void SetPixel(int x, int y, CvScalar color)
        {
            int pixelSize = imageInfo.nChannels * (imageInfo.depth / 8);
            int offset = pixelSize * x + imageInfo.widthStep * y;
            IntPtr address = new IntPtr(imageInfo.imageData.ToInt32() + offset);

            Marshal.WriteByte(address, 0, (byte)Math.Round(color.item1));
            Marshal.WriteByte(address, 1, (byte)Math.Round(color.item2));
            Marshal.WriteByte(address, 2, (byte)Math.Round(color.item3));
        }

        public IplImage CloneGray()
        {
            IplImage newImage = new IplImage(Size, Depth, 1);
            ConvertColor(newImage, Cv.CV_BGR2GRAY);
            return newImage;
        }

        public IplImage GaussianSmooth()
        {
            const int DefaultSmooth = 3;
            return GaussianSmooth(DefaultSmooth);
        }

        public IplImage GaussianSmooth(int size1)
        {
            IntPtr smoothedImage = CreateImage(Size, Depth, Channels);
            Cv.cvSmooth(Handle, smoothedImage, Cv.CV_GAUSSIAN,
                size1, 0, 0, 0);
            return new IplImage(smoothedImage, true);
        }

        public void SetZero()
        {
            CxCore.cvSetZero(Handle);
        }

        public void CopyTo(IplImage destination)
        {
            CxCore.cvCopy(Handle, destination.Handle, IntPtr.Zero);
        }

        public void CopyTo(IplImage destination, IplImage mask)
        {
            CxCore.cvCopy(Handle, destination.Handle, mask.Handle);
        }

        public void ConvertColor(IplImage destination, int code)
        {
            Cv.cvCvtColor(Handle, destination.Handle, code);
        }

        public static void Copy(IplImage source, IplImage destination)
        {
            CxCore.cvCopy(source.Handle, destination.Handle, IntPtr.Zero);
        }

        public static void Copy(IplImage source, IplImage destination, IplImage mask)
        {
            CxCore.cvCopy(source.Handle, destination.Handle, mask.Handle);
        }

        public static void Not(IplImage source, IplImage destination)
        {
            CxCore.cvNot(source.Handle, destination.Handle);
        }

        public static void Flip(IplImage source, IplImage destination, int mode)
        {
            CxCore.cvFlip(source.Handle, destination.Handle, mode);
        }

        public IplImage CloneImage()
        {
            IntPtr newImage = CxCore.cvCloneImage(Handle);
            return new IplImage(newImage, true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (ownHandle)
            {
                using (IntPtrSafeMemoryBox releasePad = new IntPtrSafeMemoryBox())
                {
                    releasePad.Value = Handle;
                    CxCore.cvReleaseImage(releasePad.Pointer);
                }
            }
        }

        public object Clone()
        {
            return CloneImage();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ImageInfo
        {
            public int  nSize;         /* sizeof(IplImage) */
            public int ID;            /* version (=0)*/
            public int nChannels;     /* Most of OpenCV functions support 1,2,3 or 4 channels */
            public int alphaChannel;  /* ignored by OpenCV */
            public int depth;         /* pixel depth in bits: IPL_DEPTH_8U, IPL_DEPTH_8S, IPL_DEPTH_16U,
                                   IPL_DEPTH_16S, IPL_DEPTH_32S, IPL_DEPTH_32F and IPL_DEPTH_64F are supported */
            public uint ignoredColorModel;
            public char ignoredChannelSeq;
            public int dataOrder;     /* 0 - interleaved color channels, 1 - separate color channels.
                                   cvCreateImage can only create interleaved images */
            public int origin;        /* 0 - top-left origin,
                                   1 - bottom-left origin (Windows bitmaps style) */
            public int align;         /* Alignment of image rows (4 or 8).
                                   OpenCV ignores it and uses widthStep instead */
            public int width;         /* image width in pixels */
            public int height;        /* image height in pixels */
            public IntPtr roi;         /* image ROI. when it is not NULL, this specifies image region to process */
            public IntPtr maskROI;
            public IntPtr imageId;
            public IntPtr tileInfo;
            public int imageSize;     /* image data size in bytes
                                   (=image->height*image->widthStep
                                   in case of interleaved data)*/
            public IntPtr imageData;   /* pointer to aligned image data */
            public int widthStep;     /* size of aligned image row in bytes */
            /* more data present. Ignoring ... */
        }
    }
}
