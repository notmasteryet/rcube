using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using OpenCVProxy.Interop;

namespace OpenCVProxy.Gui
{
    public class HighGuiTrackbar : CvUnmanagedObject
    {
        string name;
        HighGuiWindow window;

        OnTrackbar onTrackbarHolder;
        Int32SafeMemoryBox position = new Int32SafeMemoryBox();

        public event EventHandler PositionChanged;

        public string Name
        {
            get { return name; }
        }

        public HighGuiWindow Window
        {
            get { return window; }
        }

        public int Position
        {
            get { return position.Value; }
        }

        public HighGuiTrackbar(HighGuiWindow window, string name, int initial, int limit)
        {
            this.window = window;
            this.name = name;
            position.Value = initial;

            onTrackbarHolder = new OnTrackbar(OnTrackbarCallback);
            int wnd = HighGui.cvCreateTrackbar(name, window.Name, position.Pointer, limit,
                Marshal.GetFunctionPointerForDelegate(onTrackbarHolder));
            SetHandle(new IntPtr(wnd));
        }

        internal void OnTrackbarCallback(int value)
        {
            if (PositionChanged != null)
                PositionChanged(this, EventArgs.Empty);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void OnTrackbar(int value);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                position.Dispose();
            }
        }

    }
}
