using System;
using System.Collections.Generic;
using System.Text;

namespace RCube
{
    class Cube3DMap
    {        
        public IEnumerable<SidePieceInfo> GetColoredPieces()
        {
            for (int i = 0; i < 27; i++)
            {
                int x = CubeCoordinates[i, 0];
                int y = CubeCoordinates[i, 1];
                int z = CubeCoordinates[i, 2];

                int[,] assigment = CubeFacesAssigment[i];
                for (int j = 0; j < assigment.GetLength(0); j++)
                {
                    int side = assigment[j, 0];

                    int[,] coords = new int[4, 3];
                    for (int q = 0; q < 4; q++)
                    {
                        coords[q, 0] = x + CubeFacesCoordinates[side, q, 0] - 1;
                        coords[q, 1] = y + CubeFacesCoordinates[side, q, 1] - 1;
                        coords[q, 2] = z + CubeFacesCoordinates[side, q, 2] - 1;
                    }
                    yield return new SidePieceInfo(coords,  side,  assigment[j, 1], i);
                }
            }
        }

        public IEnumerable<SidePieceInfo> GetAllPieces()
        {
            for (int i = 0; i < 27; i++)
            {
                if (i == 13) continue;

                int x = CubeCoordinates[i, 0];
                int y = CubeCoordinates[i, 1];
                int z = CubeCoordinates[i, 2];

                int[,] assigment = CubeFacesAssigment[i];
                int j = 0;
                for (int side = 0; side < 6; side++)
                {
                    int[,] coords = new int[4, 3];
                    for (int q = 0; q < 4; q++)
                    {
                        coords[q, 0] = x + CubeFacesCoordinates[side, q, 0] - 1;
                        coords[q, 1] = y + CubeFacesCoordinates[side, q, 1] - 1;
                        coords[q, 2] = z + CubeFacesCoordinates[side, q, 2] - 1;
                    }

                    if (j < assigment.GetLength(0) && assigment[j, 0] == side)
                    {
                        yield return new SidePieceInfo(coords, side, assigment[j, 1], i);
                        j++;
                    }
                    else
                        yield return new SidePieceInfo(coords, side, -1, i);
                }
            }
        }

        private static int[, ,] CubeFacesCoordinates = {
            {{2,2,2}, {2,0,2}, {0,0,2}, {0,2,2}},
            {{2,2,2}, {0,2,2}, {0,2,0}, {2,2,0}},
            {{2,2,2}, {2,2,0}, {2,0,0}, {2,0,2}},
            {{0,0,0}, {2,0,0}, {2,2,0}, {0,2,0}}, 
            {{0,0,0}, {0,0,2}, {2,0,2}, {2,0,0}},
            {{0,0,0}, {0,2,0}, {0,2,2}, {0,0,2}},
        };

        private static int[,] CubeCoordinates = {
            {-2,-2,-2}, {0,-2,-2}, {2,-2,-2}, 
            {-2,-2,0}, {0,-2,0}, {2,-2,0}, 
            {-2,-2,2}, {0,-2,2}, {2,-2,2},
            {-2,0,-2}, {0,0,-2}, {2,0,-2}, 
            {-2,0,0}, {0,0,0}, {2,0,0}, 
            {-2,0,2}, {0,0,2}, {2,0,2},
            {-2,2,-2}, {0,2,-2}, {2,2,-2}, 
            {-2,2,0}, {0,2,0}, {2,2,0}, 
            {-2,2,2}, {0,2,2}, {2,2,2}
        };

        private static int[][,] CubeFacesAssigment = {
            new int[,] {{3,0}, {4,0}, {5,0}},
            new int[,] {{3,1}, {4,7}},
            new int[,] {{2,4}, {3,2}, {4,6}},
            new int[,] {{4,1}, {5,7}},
            new int[,] {{4,8}},
            new int[,] {{2,5}, {4,5}},
            new int[,] {{0,4}, {4,2}, {5,6}},
            new int[,] {{0,3}, {4,3}},
            new int[,] {{0,2}, {2,6}, {4,4}},
            new int[,] {{3,7}, {5,1}},
            new int[,] {{3,8}},
            new int[,] {{2,3}, {3,3}},
            new int[,] {{5,8}},
            new int[0,0],
            new int[,] {{2,8}},
            new int[,] {{0,5}, {5,5}},
            new int[,] {{0,8}},
            new int[,] {{0,1}, {2,7}},
            new int[,] {{1,4}, {3,6}, {5,2}},
            new int[,] {{1,5}, {3,5}},
            new int[,] {{1,6}, {2,2}, {3,4}},
            new int[,] {{1,3}, {5,3}},
            new int[,] {{1,8}},
            new int[,] {{1,7}, {2,1}},
            new int[,] {{0,6}, {1,2}, {5,4}},
            new int[,] {{0,7}, {1,1}},
            new int[,] {{0,0}, {1,0}, {2,0}}
        };

        public static int[,] SideCubes = {
            {26,17,8,7,6,15,24,25,16},
            {26,25,24,21,18,19,20,23,22},
            {26,23,20,11,2,5,8,17,14}, 
            {0,1,2,11,20,19,18,9,10},
            {0,3,6,7,8,5,2,1,4},
            {0,9,18,21,24,15,6,3,12}
        };
    }

    public struct SidePieceInfo
    {
        public int[,] Coords;
        public int Side;
        public int Piece;
        public int Cube;

        public SidePieceInfo(int[,] coords, int side, int piece, int cube)
        {
            Coords = coords;
            Side = side;
            Piece = piece;
            Cube = cube;
        }
    }
}
