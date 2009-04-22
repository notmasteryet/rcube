using System;
using System.Collections.Generic;
using System.Text;

using OpenCVProxy;
using OpenCVProxy.Interop;

namespace RCubeCapture
{
    class SquareDetector
    {
        static CvMemoryStorage storage = new CvMemoryStorage();        

        public static List<CvPoint> FindRectangles(IplImage image)
        {
            storage.Clear();

            const int N = 11;
            const int thresh = 50;

            CvSize imageSize = image.Size;
            CvSize evenImageSize = new CvSize(imageSize.width & ~1, imageSize.height & ~1);
            double MinArea = (evenImageSize.height / 12) * (evenImageSize.height / 12);
            double MaxArea = (evenImageSize.height / 4) * (evenImageSize.height / 4);

            List<CvPoint> squares = new List<CvPoint>();

            using (IplImage timg = new IplImage(imageSize, 8, 3),
                gray = new IplImage(evenImageSize, 8, 1),
                pyr = new IplImage(new CvSize(evenImageSize.width / 2, evenImageSize.height / 2), 8, 3))
            {
                Cv.cvCvtColor(image, timg, Cv.CV_BGR2HSV);

                CxCore.cvSetImageROI(timg, new CvRect(new CvPoint(), evenImageSize));

                Cv.cvPyrDown(timg, pyr, 7);
                Cv.cvPyrUp(pyr, timg, 7);
                IplImage tgray = new IplImage(evenImageSize, 8, 1);

                for (int c = 0; c < 3; c++)
                {
                    CxCore.cvSetImageCOI(timg, c + 1);
                    timg.CopyTo(tgray);

                    for (int l = 0; l < N; l++)
                    {
                        if (l == 0)
                        {
                            Cv.cvCanny(tgray, gray, 0, thresh, 5);
                            Cv.cvDilate(gray, gray, IntPtr.Zero, 1);
                        }
                        else
                        {
                            Cv.cvThreshold(tgray, gray, (l + 1) * 255 / N, 255, 0);
                        }

                        CvSeqNavigator contours;
                        using (IntPtrSafeMemoryBox box = new IntPtrSafeMemoryBox())
                        {
                            Cv.cvFindContours(gray, storage, box.Pointer, CvTypeSizes.CvContourSize,
                                Cv.CV_RETR_LIST, Cv.CV_CHAIN_APPROX_SIMPLE, new CvPoint());
                            contours = new CvSeqNavigator(box.Value);
                        }

                        while (!contours.IsEmpty)
                        {
                            IntPtr resultSeq = Cv.cvApproxPoly(contours.Pointer,
                                CvTypeSizes.CvContourSize, storage, Cv.CV_POLY_APPROX_DP,
                                Cv.cvContourPerimeter(contours) * 0.02, 0);

                            CvSeqCollection<CvPoint> result = new CvSeqCollection<CvPoint>(resultSeq, true);
                            if (result.Count == 4)
                            {
                                double area = Math.Abs(Cv.cvContourArea(resultSeq, CvSlice.WholeSeq));
                                if (area >= MinArea && area <= MaxArea &&
                                    Cv.cvCheckContourConvexity(resultSeq) != 0)
                                {
                                    CvPoint[] resultArray = result.ToArray();

                                    double dist1 = Math.Sqrt(CvPoint.Distance2(resultArray[0], resultArray[1]));
                                    double dist2 = Math.Sqrt(CvPoint.Distance2(resultArray[1], resultArray[2]));
                                    double dist3 = Math.Sqrt(CvPoint.Distance2(resultArray[2], resultArray[3]));
                                    double dist4 = Math.Sqrt(CvPoint.Distance2(resultArray[3], resultArray[0]));

                                    bool found = Math.Abs((dist1 - dist3) / (dist1 + dist3)) < 0.1 &&
                                        Math.Abs((dist2 - dist4) / (dist2 + dist4)) < 0.1 &&
                                        Math.Abs((dist1 - dist2) / (dist1 + dist2)) < 0.1;

                                    if (found)
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            squares.Add(result[i]);
                                        }
                                    }
                                }
                            }

                            contours.Next();
                        }
                    }
                }
            }
            return squares;
        }
    }
}
