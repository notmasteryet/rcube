using System;
using System.Collections.Generic;
using System.Text;

using OpenCVProxy;
using OpenCVProxy.Interop;

namespace RCubeCapture
{
    class PointTracker
    {
        static TrackPointContext context0, context1;
        static ByteSafeMemoryBox status;
        static TrackedPoint[] trackedPoints;
        static bool needToInitialize = true;
        static int flags = 0;
        static CvTermCriteria defaultTermCriteria = new CvTermCriteria(20, 0.03);
        const int MaxPointsCount = 500;
        const int WinSize = 10;

        public static bool HasData
        {
            get { return !needToInitialize && trackedPoints != null; }
        }

        public static TrackedPoint[] GetTrackedPoints()
        {
            if (!HasData) return null;

            return trackedPoints;
        }

        public static void InitializeTrackPoints(IplImage image)
        {
            status = new ByteSafeMemoryBox(MaxPointsCount);

            CvSize imageSize = image.Size;
            context0 = new TrackPointContext(imageSize, MaxPointsCount);
            context1 = new TrackPointContext(imageSize, MaxPointsCount);
        }

        private static bool InPoints(CvPoint p, IList<CvPoint> points, int index, int count)
        {
            for (int j = 0; j < count; j++)
            {
                if (CvPoint.Distance2(p, points[index + j]) < 25) return true;
            }
            return false;
        }

        public static void TrackPoints(IplImage image, List<CvPoint> rects)
        {
            if (rects.Count == 0)
            {
                needToInitialize = true;
                return;
            }

            CvSize imageSize = image.Size;
            image.ConvertColor(context1.Gray, Cv.CV_BGR2GRAY);

            if (needToInitialize)
            {
                using (IplImage eig = new IplImage(imageSize, 32, 1),
                    temp = new IplImage(imageSize, 32, 1))
                {
                    int k = 0;
                    for (int i = 0; i < rects.Count; i++)
                    {
                        CvPoint p = rects[i];
                        if (!InPoints(p, rects, 0, i - 1))
                            context1.Points[k++] = new CvPoint2D32f(p.x, p.y);
                    }
                    context1.Count = k;

                    Cv.cvFindCornerSubPix(context1.Gray, context1.Points.Pointer, context1.Count, new CvSize(WinSize, WinSize),
                        new CvSize(-1, -1), defaultTermCriteria);
                }
            }
            else if (context0.Count > 0)
            {
                Cv.cvCalcOpticalFlowPyrLK(context0.Gray, context1.Gray, context0.Pyramid, context1.Pyramid,
                    context0.Points.Pointer, context1.Points.Pointer,
                    context0.Count, new CvSize(WinSize, WinSize), 3,
                    status.Pointer, IntPtr.Zero, new CvTermCriteria(20, 0.03), flags);
                flags |= Cv.CV_LKFLOW_PYR_A_READY;

                bool[] inTracking = new bool[rects.Count];
                List<CvPoint> pointsInTrack = new List<CvPoint>();
                List<TrackedPoint> trackedPoints = new List<TrackedPoint>();
                int k = 0;
                for (int i = 0; i < context0.Count; i++)
                {
                    if (status[i] == 0) continue;

                    CvPoint2D32f sp = context1.Points[i];
                    bool valid = false;
                    CvPoint p = sp.ToCvPoint();
                    for (int j = 0; j < rects.Count; j++)
                    {
                        if (CvPoint.Distance2(p, rects[j]) < 25)
                        {
                            inTracking[j] = true;
                            valid = true;
                        }
                    }

                    if (!valid && context0.IsFading[i]) continue;                    

                    pointsInTrack.Add(p);
                    
                    context1.Points[k] = sp;
                    context1.IsFading[k] = !valid;
                    ++k;
                    TrackedPoint tp = new TrackedPoint();
                    tp.p = sp;
                    CvPoint2D32f sp0 = context0.Points[i];
                    tp.offset = new CvPoint2D32f(sp.x - sp0.x, sp.y - sp0.y);
                    tp.isNewPoint = false;
                    trackedPoints.Add(tp);
                }
                int l = k;
                for (int i = 0; i < rects.Count; i++)
                {
                    CvPoint p = rects[i];
                    if (!inTracking[i] && !InPoints(p, pointsInTrack, 0, pointsInTrack.Count))
                    {
                        pointsInTrack.Add(rects[i]);
                        CvPoint2D32f sp = new CvPoint2D32f(p.x, p.y);
                        context1.Points[k++] = sp;

                        TrackedPoint tp = new TrackedPoint();
                        tp.p = sp;
                        tp.isNewPoint = true;
                        trackedPoints.Add(tp);
                    }
                }
                if (l < k)
                {
                    Cv.cvFindCornerSubPix(context1.Gray,
                        new IntPtr(context1.Points.Pointer.ToInt32() + 4 * l), k - l, new CvSize(WinSize, WinSize),
                        new CvSize(-1, -1), defaultTermCriteria);
                }
                context1.Count = k;
                PointTracker.trackedPoints = trackedPoints.ToArray();
            }
            else
            {
                context1.Count = 0;
                PointTracker.trackedPoints = null;
            }

            if (context1.Count == 0)
            {
                needToInitialize = true;
                return;
            }

            TrackPointContext c = context1; context1 = context0; context0 = c;
            needToInitialize = false;
        }

        public static bool GetMovement(out MovementType type, out double step)
        {
            step = 0;
            type = MovementType.Nothing;

            if (HasData)
            {
                List<double> angles = new List<double>();
                List<double> distances = new List<double>();
                double sumX = 0;
                double sumY = 0;

                for (int i = 0; i < trackedPoints.Length; i++)
                {
                    if (!trackedPoints[i].isNewPoint)
                    {
                        CvPoint2D32f p = trackedPoints[i].offset;

                        double distance = Math.Sqrt(p.x * p.x + p.y * p.y);
                        distances.Add(distance);

                        if(distance < 1e-5) continue;

                        CvPoint2D32f p0 = trackedPoints[i].offset;
                        sumX += p0.x;
                        sumY += p0.y;                        

                        double angle = Math.Atan2(p.y, p.x);
                        angles.Add(angle);
                    }
                }
                if (angles.Count > 2)
                {
                    angles.Sort();
                    distances.Sort();
                    NormalizeAngles(angles);

                    double angleDiff = angles[angles.Count - 1] - angles[0];
                    double distanceDiff = distances[distances.Count - 1] - angles[0];

                    if (angleDiff < 0.25)
                    {
                        step = distanceDiff;
                        if (-Math.PI / 4 <= angles[0] && angles[0] < Math.PI  / 4)
                        {
                            type = MovementType.ToRight;
                        }
                        else if (Math.PI / 4 <= angles[0] && angles[0] < Math.PI * 3 / 4)
                        {
                            type = MovementType.ToDown;
                        }
                        else if (-Math.PI * 3 / 4 <= angles[0] && angles[0] < -Math.PI / 4)
                        {
                            type = MovementType.ToUp;
                        }
                        else
                        {
                            type = MovementType.ToLeft;
                        }
                    }
                    else
                    {
                        sumX /= angles.Count;
                        sumY /= angles.Count;


                    }
                }
            }

            return type != MovementType.Nothing;
        }

        private static void CalcMeanAndDeviation(IList<double> x, out double mean, out double deviation)
        {
            int n = x.Count;
            double sum = 0;

            for (int i = 0; i < n; i++)
            {
                sum += x[i];
            }

            mean = sum / n;

            sum = 0;
            for (int i = 0; i < n; i++)
            {
                double diff = x[i] - mean;
                sum += diff * diff;
            }

            deviation = Math.Sqrt(sum / n);
        }

        private static void NormalizeAngles(List<double> angles)
        {
            int k = angles.Count;
            while (k > 0 && (angles[angles.Count - 1] - angles[0]) > Math.PI)
            {
                double angle = angles[0] + 2 * Math.PI;
                angles.RemoveAt(0);
                angles.Add(angle);
                k--;
            }
        }
    }




}
