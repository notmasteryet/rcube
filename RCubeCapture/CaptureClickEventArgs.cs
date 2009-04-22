using System;
using System.Collections.Generic;
using System.Text;

namespace RCubeCapture
{
    public delegate void CaptureClickEventHandler(object sender, CaptureClickEventArgs e);

    public sealed class CaptureClickEventArgs : EventArgs
    {
        private int[,] colors;

        public int[,] Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        public CaptureClickEventArgs(int[,] colors)
        {
            this.colors = colors;
        }
    }
}
