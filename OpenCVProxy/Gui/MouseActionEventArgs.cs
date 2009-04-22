using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCVProxy.Gui
{
    public delegate void MouseActionEventHandler(object sender, MouseActionEventArgs e);

    public class MouseActionEventArgs : EventArgs
    {
        public readonly int EventType;
        public readonly int X;
        public readonly int Y;
        public readonly int Flags;

        public MouseActionEventArgs(int eventType, int x, int y, int flags)
        {
            this.EventType = eventType;
            this.X = x;
            this.Y = y;
            this.Flags = flags;
        }
    }
}
