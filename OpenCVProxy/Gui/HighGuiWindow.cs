using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using OpenCVProxy.Interop;

namespace OpenCVProxy.Gui
{
    public sealed class HighGuiWindow : CvUnmanagedObject 
    {
        string name;
        MouseActionEventHandler mouseAction = null;
        OnMouseCallback onMouseCallbackHolder = null;

        public string Name
        {
            get { return name; }
        }

        public event MouseActionEventHandler MouseAction
        {
            add
            {
                EnsureOnMouseCallback();
                mouseAction += value;
            }
            remove
            {
                mouseAction -= value;
            }
        }

        private void EnsureOnMouseCallback()
        {
            if (onMouseCallbackHolder == null)
            {
                onMouseCallbackHolder = new OnMouseCallback(OnMouse);
                HighGui.cvSetMouseCallback(Name,
                    Marshal.GetFunctionPointerForDelegate(onMouseCallbackHolder), IntPtr.Zero);
            }
        }

        public HighGuiWindow(string name) : this(name, HighGui.CV_WINDOW_AUTOSIZE)
        {
        }

        public HighGuiWindow(string name, int flags) 
            : base(CreateNamedWindow(name, flags))
        {
            this.name = name;
        }

        private static IntPtr CreateNamedWindow(string name, int flags)
        {
            int wnd = HighGui.cvNamedWindow(name, flags);
            return new IntPtr(wnd);
        }

        public void ShowImage(IplImage image)
        {
            HighGui.cvShowImage(Name, image.Handle);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            HighGui.cvDestroyWindow(Name);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void OnMouseCallback(int eventType, int x, int y, int flags, IntPtr param);

        private void OnMouse(int eventType, int x, int y, int flags, IntPtr param)
        {
            if (mouseAction != null)
            {
                MouseActionEventArgs e = new MouseActionEventArgs(eventType, x, y, flags);
                mouseAction(this, e);
            }
        }
    }

}
