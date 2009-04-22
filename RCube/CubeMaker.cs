using System;
using System.Collections.Generic;
using System.Text;

namespace RCube
{
    /// <summary>
    /// Algorithm that makes cube from specified set of sides.
    /// </summary>
    static class CubeMaker
    {
        static int[] permSet = { 0, 1, 2, 7, 8, 3, 6, 5, 4 };
        static int[] permRotate = { 2, 3, 4, 5, 6, 7, 0, 1, 8 };

        public delegate void MakingProgressCallback(int percentage);

        public static int[,] Make(int[,] input, MakingProgressCallback callback, out bool moreThanOne)
        {
            Context c = new Context();
            c.callback = callback;

            Dictionary<int, int> defined = new Dictionary<int, int>();
            c.sides = new List<int[]>[6];
            for (int i = 0; i < 6; i++)
            {
                c.sides[i] = new List<int[]>();
            }
            for (int i = 0; i < input.GetLength(0); i++)
            {
                int[] side = new int[9];

                for (int j = 0; j < 9; j++)
                {
                    side[permSet[j]] = input[i, j];
                }
                int sideId = 0;
                for (int j = 0; j < 9; j++)
                {
                    sideId = sideId * 9 + side[j];
                }
                if (defined.ContainsKey(sideId)) continue;

                int index = side[8];
                c.sides[index].Add(side);
                defined.Add(sideId, i);

                for (int q = 0; q < 3; q++)
                {
                    int[] newSide = new int[9];
                    sideId = 0;
                    for (int j = 0; j < 9; j++)
                    {
                        newSide[permRotate[j]] = side[j];
                    }
                    side = newSide;
                    for (int j = 0; j < 9; j++)
                    {
                        sideId = sideId * 9 + side[j];
                    }

                    if (defined.ContainsKey(sideId)) continue;

                    c.sides[index].Add(side);
                    defined.Add(sideId, i);
                }
            }

            c.Rec0();

            if (c.solution == null)
            {
                moreThanOne = false;
                return null;
            }
            else
            {
                int[,] cube = new int[6, 9];
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        cube[i, j] = c.solution[i][j];
                    }
                }
                moreThanOne = c.moreThanOne;
                return cube;
            }
        }

        private class Context
        {
            public MakingProgressCallback callback;
            public int[][] current = new int[6][];
            public int[][] solution = null;
            public List<int[]>[] sides;
            public bool moreThanOne = false;

            public int[] jobParts = new int[3]; // 2,4,3

            public void SetPercentage()
            {
                int p = jobParts[0] * 12 + jobParts[1] * 3 + jobParts[2];

                if (callback != null) callback(p * 100 / 24);
            }

            public void Rec0()
            {
                for (int i = 0; i < sides[0].Count; i++)
                {
                    current[0] = sides[0][i];
                    Rec1();
                }
            }

            void Rec1()
            {
                for (int q = 1; q <= 2; q++)
                {
                    jobParts[0] = q - 1;

                    for (int i = 0; i < sides[q].Count; i++)
                    {
                        current[1] = sides[q][i];

                        if (current[0][7] == current[1][1] ||
                            current[0][0] == current[1][0] ||
                            current[0][6] == current[1][2])
                            continue;

                        Rec2(q);
                    }
                }
            }

            void Rec2(int sel1)
            {
                for (int q = sel1 + 1; q < 6; q++)
                {
                    jobParts[1] = q - 2;

                    for (int i = 0; i < sides[q].Count; i++)
                    {
                        current[2] = sides[q][i];

                        if (current[0][1] == current[2][7] ||
                            current[0][0] == current[2][0] ||
                            current[0][2] == current[2][6] ||
                            current[1][7] == current[2][1] ||
                            current[1][0] == current[2][0] ||
                            current[1][6] == current[2][2])
                            continue;

                        Rec3(sel1, q);
                    }
                }
            }

            void Rec3(int sel1, int sel2)
            {
                for (int q = sel1 == 1 ? sel1 + 1 : 1; q < 6; q++)
                {
                    if (q == sel2) continue;

                    jobParts[2] = q == 1 ? 0 : q - 3;

                    SetPercentage();

                    for (int i = 0; i < sides[q].Count; i++)
                    {
                        current[3] = sides[q][i];

                        if (current[1][4] == current[3][6] ||
                            current[1][5] == current[3][5] ||
                            current[1][6] == current[3][4] ||
                            current[2][2] == current[3][4] ||
                            current[2][3] == current[3][3] ||
                            current[2][4] == current[3][2])
                            continue;

                        Rec4(sel1, sel2, q);
                    }

                    if (q == 1) break;
                }
            }

            void Rec4(int sel1, int sel2, int sel3)
            {
                for (int q = sel1 + 1; q < 6; q++)
                {
                    if (q == sel2 || q == sel3) continue;

                    for (int i = 0; i < sides[q].Count; i++)
                    {
                        current[4] = sides[q][i];

                        if (current[2][4] == current[4][6] ||
                            current[2][5] == current[4][5] ||
                            current[2][6] == current[4][4] ||
                            current[0][2] == current[4][4] ||
                            current[0][3] == current[4][3] ||
                            current[0][4] == current[4][2] ||
                            current[3][0] == current[4][0] ||
                            current[3][1] == current[4][7] ||
                            current[3][2] == current[4][6])
                            continue;

                        Rec5(15 - sel1 - sel2 - sel3 - q);
                    }
                }
            }
            void Rec5(int q)
            {
                for (int i = 0; i < sides[q].Count; i++)
                {
                    current[5] = sides[q][i];

                    if (current[0][4] == current[5][6] ||
                        current[0][5] == current[5][5] ||
                        current[0][6] == current[5][4] ||
                        current[1][2] == current[5][4] ||
                        current[1][3] == current[5][3] ||
                        current[1][4] == current[5][2] ||
                        current[4][0] == current[5][0] ||
                        current[4][1] == current[5][7] ||
                        current[4][2] == current[5][6] ||
                        current[3][0] == current[5][0] ||
                        current[3][7] == current[5][1] ||
                        current[3][6] == current[5][2]) continue;

                    Validate();
                }
            }

            void Validate()
            {
                int[,] sideCube = new int[6, 6];
                for (int i = 0; i < 3; i++)
                {
                    sideCube[i, i] = 1;
                    sideCube[i + 3, i + 3] = 1;
                    sideCube[current[i][8], current[i + 3][8]] = 1;
                    sideCube[current[i + 3][8], current[i][8]] = 1;
                }
                foreach (int[] c in new int[][] {
                new int[] {0,7,1,1}, 
                new int[] {0,1,2,7}, 
                new int[] {0,5,5,5}, 
                new int[] {0,3,4,3}, 
                new int[] {3,7,5,1}, 
                new int[] {3,1,4,7}, 
                new int[] {3,5,1,5}, 
                new int[] {3,3,2,3}, 
                new int[] {1,7,2,1}, 
                new int[] {1,3,5,3}, 
                new int[] {4,1,5,7}, 
                new int[] {4,5,2,5}})
                {
                    sideCube[current[c[0]][c[1]], current[c[2]][c[3]]] = 2;
                    sideCube[current[c[2]][c[3]], current[c[0]][c[1]]] = 2;
                }

                for (int i = 0; i < 36; i++)
                {
                    if (sideCube[i % 6, i / 6] == 0) return; // invalid
                }

                List<int[]> conners = new List<int[]>();
                conners.Add(new int[] { current[0][0], current[2][0], current[1][0] });
                conners.Add(new int[] { current[3][0], current[4][0], current[5][0] });
                conners.Add(new int[] { current[0][2], current[4][4], current[2][6] });
                conners.Add(new int[] { current[2][2], current[3][4], current[1][6] });
                conners.Add(new int[] { current[1][2], current[5][4], current[0][6] });
                conners.Add(new int[] { current[4][2], current[0][4], current[5][6] });
                conners.Add(new int[] { current[5][2], current[1][4], current[3][6] });
                conners.Add(new int[] { current[3][2], current[2][4], current[4][6] });

                conners.Add(new int[] { current[0][8], current[2][8], current[1][8] });
                conners.Add(new int[] { current[3][8], current[4][8], current[5][8] });
                conners.Add(new int[] { current[0][8], current[4][8], current[2][8] });
                conners.Add(new int[] { current[2][8], current[3][8], current[1][8] });
                conners.Add(new int[] { current[1][8], current[5][8], current[0][8] });
                conners.Add(new int[] { current[4][8], current[0][8], current[5][8] });
                conners.Add(new int[] { current[5][8], current[1][8], current[3][8] });
                conners.Add(new int[] { current[3][8], current[2][8], current[4][8] });

                List<int> connersId = new List<int>();
                foreach (int[] c in conners)
                {
                    if (c[1] < c[0] && c[1] < c[2])
                    {
                        int t = c[1]; c[1] = c[2]; c[2] = c[0]; c[0] = t;
                    }
                    else if (c[2] < c[0] && c[2] < c[1])
                    {
                        int t = c[2]; c[2] = c[1]; c[1] = c[0]; c[0] = t;
                    }

                    connersId.Add(c[0] * 36 + c[1] * 6 + c[2]);
                }
                connersId.Sort();

                for (int i = 0; i < connersId.Count; i += 2)
                {
                    if (connersId[i] != connersId[i + 1])
                        return;
                }

                if (solution == null)
                    solution = (int[][])current.Clone();
                else
                    moreThanOne = true;
            }
        }
    }
}
