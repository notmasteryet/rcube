using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCVProxy
{
    /// <summary>
    /// List of attributes
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CvAttrList
    {
        /// <summary>
        /// NULL-terminated array of (attribute_name,attribute_value) pairs
        /// </summary>
        public IntPtr attr;

        /// <summary>
        /// pointer to next chunk of the attributes list 
        /// </summary>
        public IntPtr next;

        public string[] GetAttributes()
        {
            List<string> attributes = new List<string>();
            int i = 0;
            IntPtr currentString = Marshal.ReadIntPtr(attr);
            while (currentString != IntPtr.Zero)
            {
                attributes.Add(Marshal.PtrToStringAnsi(currentString));
                i += 4;
                currentString = Marshal.ReadIntPtr(attr, i);
            }
            return attributes.ToArray();
        }

        public CvAttrList GetNext()
        {
            if (next == IntPtr.Zero) throw new InvalidOperationException("No next item");

            return (CvAttrList)Marshal.PtrToStructure(next, typeof(CvAttrList));
        }
    }
}
