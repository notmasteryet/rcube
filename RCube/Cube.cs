using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace RCube
{
    public class Cube : ICloneable
    {
        public int[,] Sides;

        public int this[int i, int j] 
        {
            get { return Sides[i, j]; }
        }

        public Cube(bool populate)
        {
            Sides = new int[6,8];
            if(populate)
            {
                Populate();
            }
        }

        private Cube(int[,] sides)
        {
            Sides = sides;
        }

        private void Populate()
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Sides[i, j] = i;
                }
            }
        }

        public void RotateSide(int side, bool reverse)
        {
            int[][,] rotations = GetRotations(side);
            for (int j = 0; j < 5; j++)
            {
                if (!reverse)
                {
                    int t = Sides[rotations[j][0, 3], rotations[j][1, 3]];
                    for (int q = 3; q > 0; q--)
                    {
                        Sides[rotations[j][0, q], rotations[j][1, q]] =
                            Sides[rotations[j][0, q - 1], rotations[j][1, q - 1]];
                    }
                    Sides[rotations[j][0, 0], rotations[j][1, 0]] = t;
                }
                else
                {
                    int t = Sides[rotations[j][0, 0], rotations[j][1, 0]];
                    for (int q = 1; q < 4; q++)
                    {
                        Sides[rotations[j][0, q - 1], rotations[j][1, q - 1]] =
                            Sides[rotations[j][0, q], rotations[j][1, q]];
                    }
                    Sides[rotations[j][0, 3], rotations[j][1, 3]] = t;
                }
            }
        }

        public int[] GetNeighbors(int side)
        {
            int[] sides = new int[4];
            for (int i = 0; i < 4; i++)
            {
                sides[i] = ROTATION_NEIGHBORS[side, i];
            }
            return sides;
        }

        public static void GetEdgeNeighbor0(int side, int piece, out int side1, out int piece1)
        {
            side1 = ROTATION_NEIGHBORS[side, piece / 2];
            if (piece == 1 || piece == 7)
                piece1 = 8 - piece;
            else
                piece1 = piece;
        }

        public static void GetCornerNeighbor0(int side, int piece, out int side1, out int piece1, out int side2, out int piece2)
        {
            side1 = ROTATION_NEIGHBORS[side, (piece / 2 + 3) % 4];
            side2 = ROTATION_NEIGHBORS[side, piece / 2];
            if (piece == 0)
                piece1 = piece2 = 0;
            else
                piece1 = (piece2 = piece % 6 + 2) % 6 + 2;
        }

        public static int[,] RotationNeighbors
        {
            get { return ROTATION_NEIGHBORS; }
        }

        public static int[,] RightSideForUpAndFront
        {
            get { return RIGHT_SIDE_FOR_UP_FRONT; }
        }

        private static int[,] ROTATION_NEIGHBORS = 
            { { 2, 4, 5, 1 }, { 0, 5, 3, 2 }, { 1, 3, 4, 0 }, { 4, 2, 1, 5 }, { 5, 0, 2, 3 }, { 3, 1, 0, 4} };

        private static int[,] ROTATION_SERIES = 
            { { 0, 2, 4, 6 }, { 1, 3, 5, 7 }, { 0, 4, 6, 2 }, { 7, 3, 5, 1 }, { 6, 2, 4, 0 } };

        private static int[,] RIGHT_SIDE_FOR_UP_FRONT = {
            {-1,5,1,-1,2,4},
            {2,-1,3,5,-1,0},
            {4,0,-1,1,3,-1},
            {-1,2,4,-1,5,1},
            {5,-1,0,2,-1,3},
            {1,3,-1,4,0,-1}};

        public static int[][,] GetRotations(int side)
        {
            int[][,] results = new int[5][,];
            for (int i = 0; i < 2; i++)
            {
                int[,] r = new int[2, 4];
                for (int j = 0; j < 4; j++)
                {
                    r[0, j] = side;
                    r[1, j] = ROTATION_SERIES[i, j];
                }
                results[i] = r;
            }
            for (int i = 0; i < 3; i++)
            {
                int[,] r = new int[2, 4];
                for (int j = 0; j < 4; j++)
                {
                    r[0, j] = ROTATION_NEIGHBORS[side, j];
                    r[1, j] = ROTATION_SERIES[i + 2, j];
                }
                results[i + 2] = r;
            }
            return results;
        }

        #region ICloneable Members

        public object Clone()
        {
            return new Cube((int[,])Sides.Clone());
        }

        #endregion

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public string[] FieldDisplay
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("      ").Append(Sides[5, 6]).Append(Sides[5, 7]).Append(Sides[5, 0]).Append("\r");
                sb.Append("      ").Append(Sides[5, 5]).Append('5').Append(Sides[5, 1]).Append("\r");
                sb.Append("      ").Append(Sides[5, 4]).Append(Sides[5, 3]).Append(Sides[5, 2]).Append("\r");

                sb.Append(Sides[0, 4]).Append(Sides[0, 5]).Append(Sides[0, 6]).Append(Sides[1, 2]).Append(Sides[1, 3]).Append(Sides[1, 4]).Append(Sides[3, 6]).Append(Sides[3, 7]).Append(Sides[3, 0]).Append(Sides[4, 0]).Append(Sides[4, 1]).Append(Sides[4, 2]).Append("\r");
                sb.Append(Sides[0, 3]).Append('0').Append(Sides[0, 7]).Append(Sides[1, 1]).Append('1').Append(Sides[1, 5]).Append(Sides[3, 5]).Append('3').Append(Sides[3, 1]).Append(Sides[4, 7]).Append('4').Append(Sides[4, 3]).Append("\r");
                sb.Append(Sides[0, 2]).Append(Sides[0, 1]).Append(Sides[0, 0]).Append(Sides[1, 0]).Append(Sides[1, 7]).Append(Sides[1, 6]).Append(Sides[3, 4]).Append(Sides[3, 3]).Append(Sides[3, 2]).Append(Sides[4, 6]).Append(Sides[4, 5]).Append(Sides[4, 4]).Append("\r");

                sb.Append("      ").Append(Sides[2, 0]).Append(Sides[2, 1]).Append(Sides[2, 2]).Append("\r");
                sb.Append("      ").Append(Sides[2, 7]).Append('2').Append(Sides[2, 3]).Append("\r");
                sb.Append("      ").Append(Sides[2, 6]).Append(Sides[2, 5]).Append(Sides[2, 4]);
                return sb.ToString().Split('\r');
            }
        }
    }

    public struct CubeNavigator
    {
        public Cube Cube;
        public int Side;
        public int Piece;

        public CubeNavigator(Cube cube, int side, int piece)
        {
            this.Cube = cube;
            this.Side = side;
            this.Piece = piece;
        }

        public CubePieceType Type
        {
            get
            {
                switch (Piece)
                {
                    case 1: case 3: case 5: case 7:
                        return CubePieceType.Edge;
                    case 0: case 2: case 4: case 6:
                        return CubePieceType.Corner;
                    default:
                        return CubePieceType.Middle;
                }
            }
        }

        public int Value
        {
            get { return this.Cube[Side, Piece]; }
        }

        public void MoveNextSide()
        {
            if (Piece >= 8) 
                throw new InvalidOperationException("Not next side for middle piece");
            else if ((Piece & 1) == 0)
            {
                // corner
                Side = Cube.RotationNeighbors[Side, (Piece / 2 + 3) % 4];
                if (Piece != 0)  Piece = (Piece % 6 + 2) % 6 + 2;
            }
            else
            {
                Side = Cube.RotationNeighbors[Side, Piece / 2];
                if (Piece == 1 || Piece == 7) Piece = 8 - Piece;
            }
        }

        public void MovePrevSide()
        {
            if (Piece >= 8)
                throw new InvalidOperationException("Not next side for middle piece");
            else if ((Piece & 1) == 0)
            {
                // corner
                Side = Cube.RotationNeighbors[Side, Piece / 2];
                if (Piece != 0) Piece = Piece % 6 + 2;
            }
            else
            {
                Side = Cube.RotationNeighbors[Side, Piece / 2];
                if (Piece == 1 || Piece == 7) Piece = 8 - Piece;
            }
        }

        public void MoveBy(int step)
        {
            Piece = (Piece + step) & 7;
        }

        public void MoveNext()
        {
            MoveBy(1);
        }

        public void MovePrev()
        {
            MoveBy(-1);
        }

        public void Move(bool nextSide, int step)
        {
            if (nextSide) MoveNextSide(); else MovePrevSide();
            MoveBy(step);
        }

        public void Move(int step, bool nextSide)
        {
            MoveBy(step);
            if (nextSide) MoveNextSide(); else MovePrevSide();
        }

        public void Move(bool nextSide, int step, bool nextSide2)
        {
            Move(nextSide, step);
            if (nextSide2) MoveNextSide(); else MovePrevSide();
        }

        public void Move(int step, bool nextSide, int step2)
        {
            Move(step, nextSide);
            MoveBy(step2);
        }
    }

    public enum CubePieceType
    {
        Edge,
        Corner,
        Middle
    }
}
