using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;

namespace RCube
{
    abstract class Cube2DDraw
    {
        public abstract Color GetSidePieceColor(int side, int piece);

        protected virtual void DrawPiece(Graphics graphics, PointF[] bounds, int side, int piece)
        {
            Color color = GetSidePieceColor(side, piece);
            graphics.FillPolygon(new SolidBrush(color), bounds);
            graphics.DrawPolygon(Pens.Black, bounds);
        }

        private const double dist = 12.0;
        private static Matrix aspect =
            new Matrix(new double[,] { { dist, 0, 0, 0 }, { 0, dist, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 0 } }, true) *
            new Matrix(new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, -1, dist }, { 0, 0, 0, 1 } }, true);
        private Matrix noRotation = new Matrix(new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } }, true);


        public PieceBounds[] Draw(Graphics graphics, RectangleF bounds)
        {
            return Draw(graphics, bounds, noRotation);
        }

        public PieceBounds[] Draw(Graphics graphics, RectangleF bounds, Matrix rotation)
        {
            List<PieceBounds> pieces = new List<PieceBounds>();

            double scale = Math.Min(bounds.Width, bounds.Height) / 2 
                / Math.Sqrt(2) / 2 / 3;

            Matrix display = 
                new Matrix(new double[,] {{scale, 0, 0, bounds.Left + bounds.Width / 2.0},
                    {0, -scale, 0, bounds.Top + bounds.Height / 2.0}, {0,0,1,0}, {0,0,0,1}}, true)
                * aspect * rotation;

            Cube3DMap map = new Cube3DMap();
            foreach(SidePieceInfo piece in map.GetColoredPieces())
            {
                PointF[] reg = new PointF[4];
                for (int i = 0; i < 4; i++)
                {
                    Matrix vector =
                        new Matrix(new double[,] { { piece.Coords[i, 0] }, { piece.Coords[i, 1] }, { piece.Coords[i, 2] }, { 1 } }, true);
                    vector = display * vector;

                    reg[i] = new PointF((float)(vector[0, 0] / vector[3, 0]), (float)(vector[1, 0] / vector[3, 0]));
                }

                double p = reg[0].X * reg[1].Y + reg[1].X * reg[2].Y + reg[2].X * reg[0].Y
                    - reg[0].Y * reg[1].X - reg[1].Y * reg[2].X - reg[2].Y * reg[0].X;

                if (p > 0)
                {
                    DrawPiece(graphics, reg, piece.Side, piece.Piece);

                    PieceBounds pieceBounds = new PieceBounds();
                    pieceBounds.Bounds = reg;
                    pieceBounds.Side = piece.Side;
                    pieceBounds.Piece = piece.Piece;
                    pieces.Add(pieceBounds);
                }
            }
            return pieces.ToArray();
        }

        public void DrawRotation(Graphics graphics, RectangleF bounds, Matrix rotation, int side, double angle)
        {
            double scale = Math.Min(bounds.Width, bounds.Height) / 2
                / Math.Sqrt(2) / 2 / 3;

            Matrix display =
                new Matrix(new double[,] {{scale, 0, 0, bounds.Left + bounds.Width / 2.0},
                    {0, -scale, 0, bounds.Top + bounds.Height / 2.0}, {0,0,1,0}, {0,0,0,1}}, true)
                * aspect * rotation;

            Dictionary<int, int> sideCubes = new Dictionary<int, int>();
            for (int i = 0; i < 9; i++)
            {
                sideCubes.Add(Cube3DMap.SideCubes[side, i], i);
            }
            Matrix sideRotation = GetRotation(side, angle);

            Cube3DMap map = new Cube3DMap();

            List<DrawJob> jobs = new List<DrawJob>();
            foreach (SidePieceInfo piece in map.GetAllPieces())
            {
                PointF[] reg = new PointF[4];
                double z = 0;
                for (int i = 0; i < 4; i++)
                {
                    Matrix vector =
                        new Matrix(new double[,] { { piece.Coords[i, 0] }, { piece.Coords[i, 1] }, { piece.Coords[i, 2] }, { 1 } }, true);
                    if (sideCubes.ContainsKey(piece.Cube))
                        vector = sideRotation * vector;
                    vector = display * vector;

                    reg[i] = new PointF((float)(vector[0, 0] / vector[3, 0]), (float)(vector[1, 0] / vector[3, 0]));
                    z += vector[2, 0];
                }
                
                double p = reg[0].X * reg[1].Y + reg[1].X * reg[2].Y + reg[2].X * reg[0].Y
                    - reg[0].Y * reg[1].X - reg[1].Y * reg[2].X - reg[2].Y * reg[0].X;
                
                if (p > 0)
                {
                    DrawJob job = new DrawJob();
                    job.reg = reg;
                    job.Z = z / 4;
                    job.Side = piece.Side;
                    job.Piece = piece.Piece;
                    jobs.Add(job);
                }
            }
            jobs.Sort();
            foreach (DrawJob job in jobs)
            {
                DrawPiece(graphics, job.reg, job.Side, job.Piece);
            }
        }

        private class DrawJob : IComparable<DrawJob>
        {
            public double Z;
            public PointF [] reg;
            public int Side;
            public int Piece;
        
            #region IComparable<DrawJob> Members

            public int  CompareTo(DrawJob other)
            {
 	            return -Comparer<double>.Default.Compare(Z, other.Z);
            }

            #endregion
        }

        private Matrix GetRotation(int side, double alpha)
        {
            if (side >= 3)
            {
                side -= 3;
                alpha = -alpha;
            }

            double rCos = Math.Cos(alpha);
            double rSin = Math.Sin(alpha);
            switch (side)
            {
                case 0:
                    return new Matrix(new double[,] { { rCos, rSin, 0, 0 }, { -rSin, rCos, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } }, true);
                case 1:
                    return new Matrix(new double[,] { { rCos, 0, -rSin, 0 }, { 0, 1, 0, 0 }, { rSin, 0, rCos, 0 }, { 0, 0, 0, 1 } }, true);
                case 2:
                    return new Matrix(new double[,] { { 1, 0, 0, 0 }, { 0, rCos, rSin, 0 }, { 0, -rSin, rCos, 0 }, { 0, 0, 0, 1 } }, true);
                default:
                    throw new ArgumentException("side");
            }
        }
    }

    class PieceBounds
    {
        public PointF [] Bounds;
        public int Side;
        public int Piece;

        public bool InPiece(float x, float y)
        {
            PointF min = Bounds[0];
            PointF max = Bounds[0];
            for (int i = 1; i < Bounds.Length; i++)
            {
                if (min.X > Bounds[i].X) min.X = Bounds[i].X;
                if (max.X < Bounds[i].X) max.X = Bounds[i].X;
                if (min.Y > Bounds[i].Y) min.Y = Bounds[i].Y;
                if (max.Y < Bounds[i].Y) max.Y = Bounds[i].Y;
            }
            if (x < min.X || x > max.X || y < min.Y || y > max.Y) return false;
            
            for (int i = 0; i < Bounds.Length; i++)
            {
                if ((x - Bounds[i].X) * (Bounds[(i + 1) % Bounds.Length].Y - Bounds[i].Y)
                    - (y - Bounds[i].Y) * (Bounds[(i + 1) % Bounds.Length].X - Bounds[i].X) > 0) return false;
            }

            return true;
        }
    }
    
}
