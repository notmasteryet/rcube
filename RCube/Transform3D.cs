using System;
using System.Collections.Generic;
using System.Text;

namespace RCube
{
    /// <summary>
    /// Utils to hanlde 3D transforms
    /// </summary>
    static class Transform3D
    {
        public static Matrix NoRotation = new Matrix(new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } }, true);

        public static Matrix GetYRotation(double alpha)
        {
            return
                new Matrix(new double[,] { { Math.Cos(alpha), 0, -Math.Sin(alpha), 0 }, { 0, 1, 0, 0 }, { Math.Sin(alpha), 0, Math.Cos(alpha), 0 }, { 0, 0, 0, 1 } }, true);
        }

        public static Matrix GetXRotation(double alpha)
        {
            return 
                new Matrix(new double[,] { {1,0,0,0}, {0, Math.Cos(alpha), Math.Sin(alpha), 0 }, { 0, -Math.Sin(alpha), Math.Cos(alpha), 0 }, { 0, 0, 0, 1 } }, true);
        }

        public static Matrix GetZRotation(double alpha)
        {
            return
                new Matrix(new double[,] { { Math.Cos(alpha), -Math.Sin(alpha), 0, 0 }, { Math.Sin(alpha), Math.Cos(alpha), 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } }, true);
        }
    }
}
