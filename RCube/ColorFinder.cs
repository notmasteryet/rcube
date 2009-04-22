using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RCube
{
    /// <summary>
    /// Algorithm of finding missing color pieces on the cube --
    /// autocomplete operation.
    /// </summary>
    static class ColorFinder
    {
        private static int[, ,] connerCubes = 
            {{{0,0},{1,0},{2,0}},
             {{5,4},{1,2},{0,6}},
             {{1,4},{5,2},{3,6}},
             {{3,4},{2,2},{1,6}},
             {{2,4},{3,2},{4,6}},
             {{4,4},{0,2},{2,6}},
             {{0,4},{4,2},{5,6}},
             {{5,0},{4,0},{3,0}}};

        private static int[, ,] edgeCubes = 
            {{{1,1}, {0,7}}, {{1,3}, {5,3}}, {{1,5}, {3,5}}, {{1,7}, {2,1}},
             {{4,1}, {5,7}}, {{4,3}, {0,3}}, {{4,5}, {2,5}}, {{4,7}, {3,1}},
             {{5,1}, {3,7}}, {{2,3}, {3,3}}, {{0,1}, {2,7}}, {{0,5}, {5,5}}};

        public static void FindColors(int[,] cube)
        {
            int[] colors = new int[6] { cube[0, 8], cube[1, 8], cube[2, 8], cube[3, 8], cube[4, 8], cube[5, 8]};
            Dictionary<int, int> colorsMap = new Dictionary<int,int>();
            colorsMap.Add(0, -1);
            for (int i = 0; i < 6; i++)
            {
                if (colors[i] > 0)
                {
                    if (colorsMap.ContainsKey(colors[i])) return; // invalid colors
                    colorsMap.Add(colors[i], i);
                }
            }

            if (colors.Length < 6) return; // not enough colors

            int[] connerCubeLocations = new int[8];
            byte [] connerCubeMaybeLocations = new byte[8];
            bool checkMaybe = false;
            for (int i = 0; i < 8; i++)
            {
                connerCubeLocations[i] = -1;
                connerCubeMaybeLocations[i] = 0;
            }
            for (int i = 0; i < 8; i++)
            {
                int j1 = colorsMap[cube[connerCubes[i, 0, 0], connerCubes[i, 0, 1]]];
                int j2 = colorsMap[cube[connerCubes[i, 1, 0], connerCubes[i, 1, 1]]];
                int j3 = colorsMap[cube[connerCubes[i, 2, 0], connerCubes[i, 2, 1]]];
                if (j1 >= 0 && j2 >= 0 && j3 >= 0)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if((connerCubes[j,0,0] == j1 && connerCubes[j,1,0] == j2 && connerCubes[j,2,0] == j3) ||
                            (connerCubes[j,0,0] == j2 && connerCubes[j,1,0] == j3 && connerCubes[j,2,0] == j1) ||
                            (connerCubes[j,0,0] == j3 && connerCubes[j,1,0] == j1 && connerCubes[j,2,0] == j2))
                        {
                            connerCubeLocations[j] = i;
                        }
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (connerCubeLocations[i] < 0)
                {
                    int j1 = colorsMap[cube[connerCubes[i, 0, 0], connerCubes[i, 0, 1]]];
                    int j2 = colorsMap[cube[connerCubes[i, 1, 0], connerCubes[i, 1, 1]]];
                    int j3 = colorsMap[cube[connerCubes[i, 2, 0], connerCubes[i, 2, 1]]];

                    if(j1 >= 0 || j2 >= 0 || j3 >= 0)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (connerCubeLocations[j] >= 0) continue;
                            if ((MaybeEqual(connerCubes[j, 0, 0], j1) && MaybeEqual(connerCubes[j, 1, 0], j2) && MaybeEqual(connerCubes[j, 2, 0], j3)) ||
                                (MaybeEqual(connerCubes[j, 0, 0], j2) && MaybeEqual(connerCubes[j, 1, 0], j3) && MaybeEqual(connerCubes[j, 2, 0], j1)) ||
                                (MaybeEqual(connerCubes[j, 0, 0], j3) && MaybeEqual(connerCubes[j, 1, 0], j1) && MaybeEqual(connerCubes[j, 2, 0], j2)))
                            {
                                connerCubeMaybeLocations[i] |= (byte)(1 << j);
                                checkMaybe = true;
                            }
                        }
                    }
                }
            }

            while (checkMaybe)
            {
                checkMaybe = false;
                for (int i = 0; i < 8; i++)
                {
                    if (connerCubeMaybeLocations[i] != 0 && ((connerCubeMaybeLocations[i] & (connerCubeMaybeLocations[i] - 1)) == 0))
                    {
                        int j1 = colorsMap[cube[connerCubes[i, 0, 0], connerCubes[i, 0, 1]]];
                        int j2 = colorsMap[cube[connerCubes[i, 1, 0], connerCubes[i, 1, 1]]];
                        int j3 = colorsMap[cube[connerCubes[i, 2, 0], connerCubes[i, 2, 1]]];

                        byte mask = connerCubeMaybeLocations[i];
                        int j = 0;
                        while (mask > (1 << j)) j++;

                        if (MaybeEqual(connerCubes[j, 0, 0], j1) && MaybeEqual(connerCubes[j, 1, 0], j2) && MaybeEqual(connerCubes[j, 2, 0], j3))
                        {
                            cube[connerCubes[i, 0, 0], connerCubes[i, 0, 1]] = colors[connerCubes[j, 0, 0]];
                            cube[connerCubes[i, 1, 0], connerCubes[i, 1, 1]] = colors[connerCubes[j, 1, 0]];
                            cube[connerCubes[i, 2, 0], connerCubes[i, 2, 1]] = colors[connerCubes[j, 2, 0]];
                        }
                        else if (MaybeEqual(connerCubes[j, 0, 0], j2) && MaybeEqual(connerCubes[j, 1, 0], j3) && MaybeEqual(connerCubes[j, 2, 0], j1))
                        {
                            cube[connerCubes[i, 0, 0], connerCubes[i, 0, 1]] = colors[connerCubes[j, 2, 0]];
                            cube[connerCubes[i, 1, 0], connerCubes[i, 1, 1]] = colors[connerCubes[j, 0, 0]];
                            cube[connerCubes[i, 2, 0], connerCubes[i, 2, 1]] = colors[connerCubes[j, 1, 0]];
                        }
                        else if (MaybeEqual(connerCubes[j, 0, 0], j3) && MaybeEqual(connerCubes[j, 1, 0], j1) && MaybeEqual(connerCubes[j, 2, 0], j2))
                        {
                            cube[connerCubes[i, 0, 0], connerCubes[i, 0, 1]] = colors[connerCubes[j, 1, 0]];
                            cube[connerCubes[i, 1, 0], connerCubes[i, 1, 1]] = colors[connerCubes[j, 2, 0]];
                            cube[connerCubes[i, 2, 0], connerCubes[i, 2, 1]] = colors[connerCubes[j, 0, 0]];
                        }

                        for (int q = 0; q < 8; q++)
                        {
                            connerCubeMaybeLocations[q] &= (byte)~mask;
                        }
                        checkMaybe = true;
                    }
                }
            }

            int[] edgeCubeLocations = new int[12];
            checkMaybe = false;
            short[] edgeCubeMaybeLocations = new short[12];
            for (int i = 0; i < 12; i++)
            {
                edgeCubeLocations[i] = -1;
                edgeCubeMaybeLocations[i] = 0;
            }

            for (int i = 0; i < 12; i++)
            {
                int j1 = colorsMap[cube[edgeCubes[i, 0, 0], edgeCubes[i, 0, 1]]];
                int j2 = colorsMap[cube[edgeCubes[i, 1, 0], edgeCubes[i, 1, 1]]];

                if (j1 >= 0 && j2 >= 0)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if ((edgeCubes[j, 0, 0] == j1 && edgeCubes[j, 1, 0] == j2) ||
                            (edgeCubes[j, 0, 0] == j2 && edgeCubes[j, 1, 0] == j1))
                        {
                            edgeCubeLocations[j] = i;
                        }
                    }
                }
            }

            for (int i = 0; i < 12; i++)
            {
                if (edgeCubeLocations[i] < 0)
                {
                    int j1 = colorsMap[cube[edgeCubes[i, 0, 0], edgeCubes[i, 0, 1]]];
                    int j2 = colorsMap[cube[edgeCubes[i, 1, 0], edgeCubes[i, 1, 1]]];

                    if (j1 >= 0 || j2 >= 0)
                    {
                        for (int j = 0; j < 12; j++)
                        {
                            if (edgeCubeLocations[j] >= 0) continue;
                            if ((MaybeEqual(edgeCubes[j, 0, 0], j1) && MaybeEqual(edgeCubes[j, 1, 0], j2)) ||
                                (MaybeEqual(edgeCubes[j, 0, 0], j2) && MaybeEqual(edgeCubes[j, 1, 0], j1)))
                            {
                                edgeCubeMaybeLocations[i] |= (short)(1 << j);
                                checkMaybe = true;
                            }

                        }
                    }
                }
            }

            while (checkMaybe)
            {
                checkMaybe = false;
                for (int i = 0; i < 12; i++)
                {
                    if (edgeCubeMaybeLocations[i] != 0 && ((edgeCubeMaybeLocations[i] & (edgeCubeMaybeLocations[i] - 1)) == 0))
                    {
                        int j1 = colorsMap[cube[edgeCubes[i, 0, 0], edgeCubes[i, 0, 1]]];
                        int j2 = colorsMap[cube[edgeCubes[i, 1, 0], edgeCubes[i, 1, 1]]];

                        short mask = edgeCubeMaybeLocations[i];
                        int j = 0;
                        while (mask > (1 << j)) j++;

                        if (MaybeEqual(edgeCubes[j, 0, 0], j1) && MaybeEqual(edgeCubes[j, 1, 0], j2))
                        {
                            cube[edgeCubes[i, 0, 0], edgeCubes[i, 0, 1]] = colors[edgeCubes[j, 0, 0]];
                            cube[edgeCubes[i, 1, 0], edgeCubes[i, 1, 1]] = colors[edgeCubes[j, 1, 0]];
                        }
                        else if (MaybeEqual(edgeCubes[j, 0, 0], j2) && MaybeEqual(edgeCubes[j, 1, 0], j1))
                        {
                            cube[edgeCubes[i, 0, 0], edgeCubes[i, 0, 1]] = colors[edgeCubes[j, 1, 0]];
                            cube[edgeCubes[i, 1, 0], edgeCubes[i, 1, 1]] = colors[edgeCubes[j, 0, 0]];
                        }

                        for (int q = 0; q < 12; q++)
                        {
                            edgeCubeMaybeLocations[q] &= (short)~mask;
                        }
                        checkMaybe = true;
                    }
                }
            }
        }

        private static bool MaybeEqual(int i, int j)
        {
            return j == -1 || i == j;
        }
    }
}
