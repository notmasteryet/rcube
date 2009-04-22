using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Media;

using OpenCVProxy;
using OpenCVProxy.Interop;
using OpenCVProxy.Gui;

namespace RCubeCapture
{
    [DefaultProperty("Title")]
    public class CubeCapture : Component
    {
        string title = "Camera";

        bool visible = false;
        HighGuiWindow window = null;
        CvCapture capture = null;
        IplImage lastFrame = null;
        Thread captureThread = null;
        bool flipImage;
        volatile bool closing = false;
        bool isBusy = false;
        int captureDelay = 2000;
        string captureSound = null;
        SoundPlayer player;
        Timer t = null;
        PieceInfo[,] lastLayout = null;

        public bool IsBusy
        {
            get { return isBusy; }
        }

        [DefaultValue(false)]
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value; 
                    OnVisibilityChanged();
                }
            }
        }

        [DefaultValue("Camera")]
        public string Title
        {
            get { return title; }
            set
            {
                if (IsBusy) throw new InvalidOperationException("Cannot change title during capturing");

                title = value;
            }
        }

        [DefaultValue(null)]
        public string CaptureSound
        {
            get { return captureSound; }
            set { captureSound = value; }
        }

        [DefaultValue(2000)]
        public int CaptureDelay
        {
            get { return captureDelay; }
            set { captureDelay = value; }
        }

        public event CaptureClickEventHandler CaptureClick;
        public event EventHandler CameraError;
        public event EventHandler Closed;

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        private void OnVisibilityChanged()
        {
            if (visible)
                ShowWindow();
            else
                HideWindow();
        }

        private void ShowWindow()
        {
            closing = false;
            captureThread = new Thread(CaptureFramesThread);
            captureThread.Name = "Camera Capture Thread";
            captureThread.Start();
        }

        private void CaptureFramesThread()
        {
            try
            {
                isBusy = true;

                using (window = new HighGuiWindow(Title))
                {
                    IntPtr hWnd = HighGui.cvGetWindowHandle(Title);

                    using (capture = new CvCapture(0))
                    {
                        IplImage frame = capture.GetNextFrame();
                        OnInitialize(frame);

                        while (true)
                        {
                            if (flipImage)
                                IplImage.Flip(frame, lastFrame, 0);
                            else
                                frame.CopyTo(lastFrame);
                           
                            ProcessFrame();

                            const int EscapeKey = 27;
                            const int InterFrameDelay = 50;

                            if (closing) break;

                            int key = HighGui.cvWaitKey(InterFrameDelay);
                            if (!IsWindowVisible(hWnd) || key == EscapeKey)
                            {
                                Hide();
                                OnClosed();
                                break;
                            }

                            frame = capture.GetNextFrame();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Camera error: " + ex.ToString());
                OnCameraError();
            }
            isBusy = false;
        }

        private void OnCameraError()
        {
            if (CameraError != null)
            {
                CameraError(this, EventArgs.Empty);
            }
        }

        private void OnClosed()
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        private void OnInitialize(IplImage frame)
        {
            lastFrame = new IplImage(frame.Size,
                CxCore.IPL_DEPTH_8U, frame.Channels);
            flipImage = frame.Origin == CxCore.IPL_ORIGIN_BL;

            PointTracker.InitializeTrackPoints(lastFrame);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr wnd);

        void ProcessFrame()
        {
            List<CvPoint> rects = SquareDetector.FindRectangles(lastFrame);

            PieceInfo[] pieces = GetPieces(rects);

            PieceInfo[,] layout = LayoutPieces(pieces);
            if (layout != null && lastLayout == null)
            {
                lastLayout = layout;
                NotifyClick();

                int[,] colors = new int[9,3];
                for (int i = 0; i < 9; i++)
                {
                    int[] color = layout[i / 3, i % 3].color;
                    colors[i, 0] = color[0];
                    colors[i, 1] = color[1];
                    colors[i, 2] = color[2];
                }

                CaptureClickEventArgs e = new CaptureClickEventArgs(colors);
                OnCaptureClick(e);
            }

            DrawSquares(rects, pieces, window);
        }

        private void OnCaptureClick(CaptureClickEventArgs e)
        {
            if (CaptureClick != null) CaptureClick(this, e);
        }

        void NotifyClick()
        {
            t = new Timer(delegate
            {
                lastLayout = null;
                t.Dispose();
                t = null;
            },  null, CaptureDelay, Timeout.Infinite);

            if (CaptureSound != null)
            {
                if (player == null)
                {
                    player = new SoundPlayer(CaptureSound);
                }
                player.Play();
            }
        }

        PieceInfo[,] LayoutPieces(PieceInfo[] pieces)
        {
            if (pieces.Length < 9) return null;

            PieceInfo[,] layout = new PieceInfo[3, 3];

            int[] sizes = new int[pieces.Length];
            for (int i = 0; i < pieces.Length; i++)
            {
                sizes[i] = pieces[i].size;
            }
            Array.Sort(sizes);

            double middleSize = sizes[pieces.Length / 2] * 5 / 4;

            int[,] index = new int[pieces.Length, 2];
            index[0, 0] = 0; index[0, 1] = 0;
            int minX = 0;
            int minY = 0;
            for (int i = 1; i < pieces.Length; i++)
            {
                index[i, 0] = (int)Math.Round((pieces[i].cx - pieces[0].cx) / middleSize);
                index[i, 1] = (int)Math.Round((pieces[i].cy - pieces[0].cy) / middleSize);

                if (index[i, 0] < minX) minX = index[i, 0];
                if (index[i, 1] < minY) minY = index[i, 1];
            }


            for (int i = 1; i < pieces.Length; i++)
            {
                int x = -minX + index[i, 0];
                int y = -minY + index[i, 1];

                if (x >= 0 && x < 3 && y >= 0 && y < 3)
                {
                    layout[y, x] = pieces[i];
                }
            }

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (layout[i, j] == null) return null;

            return layout;
        }

        PieceInfo[] GetPieces(List<CvPoint> squares)
        {
            int smoothSize = (lastFrame.Size.height / 5) | 1; // 10% and make it odd
            IplImage smoothedImage = lastFrame.GaussianSmooth(smoothSize);

            List<PieceInfo> pieces = new List<PieceInfo>();
            for (int i = 0; i < squares.Count; i += 4)
            {
                CvPoint p1 = squares[i + 0];
                CvPoint p2 = squares[i + 1];
                CvPoint p3 = squares[i + 2];
                CvPoint p4 = squares[i + 3];

                PieceInfo pi = new PieceInfo();
                pi.cx = (p1.x + p3.x) / 2;
                pi.cy = (p1.y + p3.y) / 2;
                pi.size = (int)Math.Sqrt(CvPoint.Distance2(p1, p2));

                // 1/8th to the center
                pi.color = GetAveragePixelsColor(smoothedImage, (p1.x * 7 + pi.cx) / 8, (p1.y * 7 + pi.cy) / 8, 2, 2);
                pieces.Add(pi);
            }

            return pieces.ToArray();
        }

        int[] GetAveragePixelsColor(IplImage image, int x, int y, int width, int height)
        {
            double r = 0, g = 0, b = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {                    
                    CvScalar color = image.GetPixel(x + j, y + i);
                    r += color.R;
                    g += color.G;
                    b += color.B;
                }
            }
            int pixelsCount = height * width;

            return new int[] { (int)(r / pixelsCount), (int)(g / pixelsCount), (int)(b / pixelsCount) };
        }

        void DrawSquares(List<CvPoint> squares, PieceInfo[] pieces, HighGuiWindow window)
        {
            using (IplImage cpy = lastFrame.CloneImage())
            {
                
                StructureSafeMemoryBox<CvPoint> pt = new StructureSafeMemoryBox<CvPoint>(4);
                IntPtrSafeMemoryBox rect = new IntPtrSafeMemoryBox();
                rect.Value = pt.Pointer;
                Int32SafeMemoryBox count = new Int32SafeMemoryBox();
                count.Value = 4;

                for (int i = 0; i < squares.Count; i += 4)
                {
                    pt[0] = squares[i + 0];
                    pt[1] = squares[i + 1];
                    pt[2] = squares[i + 2];
                    pt[3] = squares[i + 3];

                    CvPoint center = new CvPoint((squares[i + 0].x + squares[i + 2].x) / 2,
                        (squares[i + 0].y + squares[i + 2].y) / 2);

                    // CvScalar color = new CvScalar(0, 255, 255);
                    CxCore.cvPolyLine(cpy, rect.Pointer, count.Pointer,
                        1, 1, CvScalar.FromRGB(255, 255, 0), 3, CxCore.CV_AA, 0);
                }

                PieceInfo[,] layout = lastLayout;
                if (layout != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            pt[0] = new CvPoint(j * 20 + 3, i * 20 + 3);
                            pt[1] = new CvPoint(j * 20 + 3, i * 20 + 18);
                            pt[2] = new CvPoint(j * 20 + 18, i * 20 + 18);
                            pt[3] = new CvPoint(j * 20 + 18, i * 20 + 3);
                            CvScalar color = CvScalar.FromRGB(layout[i, j].color[0], layout[i, j].color[1], layout[i, j].color[2]);
                            CxCore.cvFillPoly(cpy, rect.Pointer, count.Pointer, 1, color, CxCore.CV_AA, 0);
                        }
                    }
                }

                count.Dispose();
                rect.Dispose();
                pt.Dispose();

                window.ShowImage(cpy);
            }
        }

        private void HideWindow()
        {
            if (captureThread == null) return;

            closing = true;
            captureThread = null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                HideWindow();
                visible = false;
            }
        }
    }
}
