using System;
using System.Collections.Generic;
using System.Text;

namespace RCube
{
    sealed class KnightsCubeSolver : CubeSolver
    {
        public override CubeSolutionGroup[] Solve(Cube cube)
        {
            List<CubeSolutionGroup> solutions = new List<CubeSolutionGroup>();
            int stage = GetCubeStage(cube);
            while (stage < 7)
            {
                switch (stage)
                {
                    case 0:
                        cube = SolveStage0(cube, solutions);
                        break;
                    case 1:
                        cube = SolveStage1(cube, solutions);
                        break;
                    case 2:
                        cube = SolveStage2(cube, solutions);
                        break;
                    case 3:
                        cube = SolveStage3(cube, solutions);
                        break;
                    case 4:
                        cube = SolveStage4(cube, solutions);
                        break;
                    case 5:
                        cube = SolveStage5(cube, solutions);
                        break;
                    case 6:
                        cube = SolveStage6(cube, solutions);
                        break;
                    default:
                        goto m1;
                    //throw new NotImplementedException();
                }

                int oldStage = stage;
                stage = GetCubeStage(cube);
                if (oldStage >= stage) throw new Exception("Bad algorithm or cube");
            }
        m1:
            foreach (CubeSolutionGroup group in solutions)
            {
                OptimizeRotations(group);
            }

            return solutions.ToArray();
        }

        private void OptimizeRotations(CubeSolutionGroup group)
        {
            List<int> rotations = new List<int>(group.rotation);

            bool check = true;
            while (check)
            {
                check = false;
                // four in row
                for (int i = 0; i < rotations.Count - 3; i++)
                {
                    if (rotations[i] == rotations[i + 1] && rotations[i] == rotations[i + 2] && rotations[i] == rotations[i + 3])
                    {
                        rotations.RemoveRange(i--, 4);
                        check = true;
                    }
                }

                // three in row
                for (int i = 0; i < rotations.Count - 2; i++)
                {
                    if (rotations[i] == rotations[i + 1] && rotations[i] == rotations[i + 2])
                    {
                        rotations.RemoveRange(i, 2);
                        rotations[i] = ~i;
                        check = true;
                    }
                }

                for (int i = 0; i < rotations.Count - 1; i++)
                {
                    if (rotations[i] == ~rotations[i + 1])
                    {
                        rotations.RemoveRange(i--, 2);
                        check = true;
                    }
                }
            }
            group.rotation = rotations.ToArray();
        }

        private Cube SolveStage6(Cube cube, List<CubeSolutionGroup> solutions)
        {
            cube = (Cube)cube.Clone();
            List<int> rotations = new List<int>();

            CubeSolutionGroup sol = new CubeSolutionGroup();
            sol.stage = 6;
            sol.StartCube = (Cube)cube.Clone();

            int topSide = (BaseSide + 3) % 6;

            CubeNavigator nav = new CubeNavigator(cube, topSide, 1);
            nav.MoveNextSide();
            int[] sides = new int[4];
            int[] values = new int[4];
            int sameIndex = -1;
            for (int i = 0; i < 4; i++)
            {
                sides[i] = nav.Side;
                values[i] = nav.Value;
                if (nav.Side == nav.Value) sameIndex = i;
                nav.Move(false, 2, true);
            }

            if (sameIndex >= 0)
            { // 7a, 7b
                nav.Move(false, 2 * sameIndex - 2, true);
                bool clockwise = sameIndex < 2 ? values[2] == sides[3] : values[0] == sides[1];
                sol.frontSide = nav.Side;
                nav.MoveBy(1);
                int sF = nav.Side;
                nav.MovePrevSide();
                int sR = nav.Side;
                int sB = (sF + 3) % 6;
                int sL = (sR + 3) % 6;

                if (clockwise)
                {
                    DoMoves(cube, sR, 2, rotations);
                    DoMoves(cube, topSide, 1, rotations);
                    DoMoves(cube, sF, 1, rotations);
                    DoMoves(cube, sB, -1, rotations);
                    DoMoves(cube, sR, 2, rotations);
                    DoMoves(cube, sF, -1, rotations);
                    DoMoves(cube, sB, 1, rotations);
                    DoMoves(cube, topSide, 1, rotations);
                    DoMoves(cube, sR, 2, rotations);
                }
                else
                {
                    DoMoves(cube, sR, 2, rotations);
                    DoMoves(cube, topSide, -1, rotations);
                    DoMoves(cube, sF, 1, rotations);
                    DoMoves(cube, sB, -1, rotations);
                    DoMoves(cube, sR, 2, rotations);
                    DoMoves(cube, sF, -1, rotations);
                    DoMoves(cube, sB, 1, rotations);
                    DoMoves(cube, topSide, -1, rotations);
                    DoMoves(cube, sR, 2, rotations);
                }
            }
            else
            { // 7c, 7d
                if (sides[0] == values[1] || sides[1] == values[2])
                { // 7d
                    if (sides[0] == values[1])
                        nav.Move(false, -2, true);

                    nav.MoveBy(1);
                    int sF = nav.Side;
                    nav.MovePrevSide();
                    int sR = nav.Side;
                    int sB = (sF + 3) % 6;
                    int sL = (sR + 3) % 6;

                    DoMoves(cube, sR, 1, rotations);
                    DoMoves(cube, sB, -1, rotations);
                    DoMoves(cube, sR, -1, rotations);
                    DoMoves(cube, sB, 1, rotations);
                    DoMoves(cube, sF, 1, rotations);
                    DoMoves(cube, sR, -1, rotations);
                    DoMoves(cube, sF, 1, rotations);
                    DoMoves(cube, sB, -1, rotations);
                    DoMoves(cube, sR, -1, rotations);
                    DoMoves(cube, sB, 1, rotations);
                    DoMoves(cube, sR, 1, rotations);
                    DoMoves(cube, sF, 2, rotations);
                    DoMoves(cube, topSide, 1, rotations);
                }
                else
                {
                    nav.MoveBy(1);
                    int sF = nav.Side;
                    nav.MovePrevSide();
                    int sR = nav.Side;
                    int sB = (sF + 3) % 6;
                    int sL = (sR + 3) % 6;

                    DoMoves(cube, sR, 1, rotations);
                    DoMoves(cube, sL, 1, rotations);
                    DoMoves(cube, topSide, 2, rotations);
                    DoMoves(cube, sR, -1, rotations);
                    DoMoves(cube, sL, -1, rotations);
                    DoMoves(cube, sF, -1, rotations);
                    DoMoves(cube, sB, -1, rotations);
                    DoMoves(cube, topSide, 2, rotations);
                    DoMoves(cube, sF, 1, rotations);
                    DoMoves(cube, sB, 1, rotations);
                }
            }
            sol.rotation = rotations.ToArray();
            solutions.Add(sol);
            return cube;
        }

        private Cube SolveStage5(Cube cube, List<CubeSolutionGroup> solutions)
        {
            cube = (Cube)cube.Clone();
            List<int> rotations = new List<int>();

            CubeSolutionGroup sol = new CubeSolutionGroup();
            sol.stage = 5;
            sol.StartCube = (Cube)cube.Clone();

            int topSide = (BaseSide + 3) % 6;

            CubeNavigator nav = new CubeNavigator(cube, topSide, 0);
            nav.MoveNextSide();
            bool isSameSideFound = false;
            for (int i = 0; i < 4; i++)
            {
                int v = nav.Value;
                nav.MoveBy(2);
                if (v == nav.Value)
                {
                    isSameSideFound = true;
                    break;
                }
                nav.MovePrevSide();
            }
            if (isSameSideFound)
            {
                int s = nav.Side;
                int t = GetMoves(s, nav.Value, topSide);
                DoMoves(cube, topSide, t, rotations);

                nav.Move(true, t * 2 - 2, false);
                sol.frontSide = nav.Side;
                int sF = nav.Side;
                nav.MovePrevSide();
                int sR = nav.Side;
                int sB = (sF + 3) % 6;
                int sL = (sR + 3) % 6;

                if (nav.Value != nav.Side)
                {
                    DoMoves(cube, sB, -1, rotations);
                    DoMoves(cube, sR, 1, rotations);
                    DoMoves(cube, sB, -1, rotations);
                    DoMoves(cube, sL, 2, rotations);
                    DoMoves(cube, sB, 1, rotations);
                    DoMoves(cube, sR, -1, rotations);
                    DoMoves(cube, sB, -1, rotations);
                    DoMoves(cube, sL, 2, rotations);
                    DoMoves(cube, sB, 2, rotations);
                    DoMoves(cube, topSide, -1, rotations);
                }
            }
            else
            {
                if (nav.Value != nav.Side)
                {
                    nav.Move(2, false);
                    if (nav.Value != nav.Side)
                    {
                        DoMoves(cube, topSide, 1, rotations);
                        if (nav.Value != nav.Side)
                        {
                            nav.Move(true, -2);
                        }
                    }
                }
                nav.MoveBy(2);
                sol.frontSide = nav.Side;
                int sF = nav.Side;
                nav.MovePrevSide();
                int sR = nav.Side;
                int sB = (sF + 3) % 6;

                DoMoves(cube, sF, 1, rotations);
                DoMoves(cube, sR, 1, rotations);
                DoMoves(cube, topSide, -1, rotations);
                DoMoves(cube, sR, -1, rotations);
                DoMoves(cube, sF, 1, rotations);
                DoMoves(cube, BaseSide, 1, rotations);
                DoMoves(cube, sR, -1, rotations);
                DoMoves(cube, sB, -1, rotations);
                DoMoves(cube, sR, -1, rotations);
                DoMoves(cube, sB, 1, rotations);
                DoMoves(cube, sR, 2, rotations);
                DoMoves(cube, BaseSide, -1, rotations);
                DoMoves(cube, sF, 2, rotations);
            }
            sol.rotation = rotations.ToArray();
            solutions.Add(sol);
            return cube;
        }

        private Cube SolveStage4(Cube cube, List<CubeSolutionGroup> solutions)
        {
            cube = (Cube)cube.Clone();
            int topSide = (BaseSide + 3) % 6;
            int f = 0;
            if (cube[topSide, 0] == topSide) f |= 1;
            if (cube[topSide, 2] == topSide) f |= 2;
            if (cube[topSide, 4] == topSide) f |= 4;
            if (cube[topSide, 6] == topSide) f |= 8;

            CubeSolutionGroup sol = new CubeSolutionGroup();
            sol.stage = 4;
            sol.StartCube = (Cube)cube.Clone();

            List<int> rotations = new List<int>();

            CubeNavigator nav = new CubeNavigator(cube, topSide, 0);
            int s1, s2;
            CubeNavigator nav2;

            switch (f)
            {
                case 3:
                    nav.MoveBy(-2);
                    goto case 6;
                case 6: // 5a, 5c
                    nav.MoveNextSide();
                    s1 = nav.Side;
                    nav.MoveNextSide();
                    s2 = nav.Side;
                    sol.frontSide = s2;

                    if (nav.Value == topSide)
                    {
                        DoMoves(cube, s1, -1, rotations);
                        DoMoves(cube, s2, -1, rotations);
                        DoMoves(cube, (s1 + 3) % 6, 1, rotations);
                        DoMoves(cube, s2, 1, rotations);
                        DoMoves(cube, s1, 1, rotations);
                        DoMoves(cube, s2, -1, rotations);
                        DoMoves(cube, (s1 + 3) % 6, -1, rotations);
                        DoMoves(cube, s2, 1, rotations);
                    }
                    else
                    {
                        DoMoves(cube, s2, 2, rotations);
                        DoMoves(cube, BaseSide, -1, rotations);
                        DoMoves(cube, s2, 1, rotations);
                        DoMoves(cube, topSide, 2, rotations);
                        DoMoves(cube, s2, -1, rotations);
                        DoMoves(cube, BaseSide, 1, rotations);
                        DoMoves(cube, s2, 1, rotations);
                        DoMoves(cube, topSide, 2, rotations);
                        DoMoves(cube, s2, 1, rotations);
                    }

                    break;
                case 12:
                    nav.MoveBy(2);
                    goto case 6;
                case 9:
                    nav.MoveBy(4);
                    goto case 6;
                case 5: // 5b
                    nav2 = nav;
                    nav2.Move(2, true);
                    if (nav2.Value != topSide)
                    {
                        nav.MoveBy(4);
                    }
                    nav.MoveNextSide();
                    s1 = nav.Side;
                    nav.MoveNextSide();
                    s2 = nav.Side;
                    sol.frontSide = s2;

                    DoMoves(cube, (s1 + 3) % 6, -1, rotations);
                    DoMoves(cube, (s2 + 3) % 6, -1, rotations);
                    DoMoves(cube, s1, -1, rotations);
                    DoMoves(cube, (s2 + 3) % 6, 1, rotations);
                    DoMoves(cube, (s1 + 3) % 6, 1, rotations);
                    DoMoves(cube, (s2 + 3) % 6, -1, rotations);
                    DoMoves(cube, s1, 1, rotations);
                    DoMoves(cube, (s2 + 3) % 6, 1, rotations);
                    break;
                case 10:
                    nav.MoveBy(2);
                    goto case 5;
                case 1:
                    nav.MoveBy(4);
                    goto case 4;
                case 2:
                    nav.MoveBy(-2);
                    goto case 4;
                case 4: // 5d, 5e
                    nav.MoveNextSide();
                    s1 = nav.Side;
                    nav.MoveNextSide();
                    s2 = nav.Side;
                    sol.frontSide = s2;

                    if (nav.Value == topSide)
                    {
                        DoMoves(cube, (s1 + 3) % 6, -1, rotations);
                        DoMoves(cube, topSide, 2, rotations);
                        DoMoves(cube, (s1 + 3) % 6, 1, rotations);
                        DoMoves(cube, topSide, 1, rotations);
                        DoMoves(cube, (s1 + 3) % 6, -1, rotations);
                        DoMoves(cube, topSide, 1, rotations);
                        DoMoves(cube, (s1 + 3) % 6, 1, rotations);
                    }
                    else
                    {
                        DoMoves(cube, s1, -1, rotations);
                        DoMoves(cube, topSide, -1, rotations);
                        DoMoves(cube, s1, 1, rotations);
                        DoMoves(cube, topSide, -1, rotations);
                        DoMoves(cube, s1, -1, rotations);
                        DoMoves(cube, topSide, 2, rotations);
                        DoMoves(cube, s1, 1, rotations);
                    }

                    break;
                case 8:
                    nav.MoveBy(2);
                    goto case 4;
                case 0: // 5f, 5g
                    nav2 = nav;
                    nav2.MoveNextSide();
                    for (int i = 0; i < 4; i++)
                    {
                        bool valid = nav2.Value == topSide;
                        nav2.MoveBy(2);
                        valid = valid && nav2.Value == topSide;
                        nav2.MovePrevSide();
                        if (valid) break;
                    }
                    nav = nav2;
                    s1 = nav.Side;
                    nav.MoveNextSide();
                    s2 = nav.Side;
                    sol.frontSide = s2;

                    nav2.MoveBy(2);
                    if (nav2.Value == topSide)
                    {
                        DoMoves(cube, (s2 + 3) % 6, 1, rotations);
                        DoMoves(cube, topSide, 2, rotations);
                        DoMoves(cube, (s2 + 3) % 6, 2, rotations);
                        DoMoves(cube, topSide, -1, rotations);
                        DoMoves(cube, (s2 + 3) % 6, 2, rotations);
                        DoMoves(cube, topSide, -1, rotations);
                        DoMoves(cube, (s2 + 3) % 6, 2, rotations);
                        DoMoves(cube, topSide, 2, rotations);
                        DoMoves(cube, (s2 + 3) % 6, 1, rotations);
                    }
                    else
                    {
                        DoMoves(cube, s1, -1, rotations);
                        DoMoves(cube, topSide, 2, rotations);
                        DoMoves(cube, s1, 1, rotations);
                        DoMoves(cube, topSide, 1, rotations);
                        DoMoves(cube, s1, -1, rotations);
                        DoMoves(cube, topSide, -1, rotations);
                        DoMoves(cube, s1, 1, rotations);
                        DoMoves(cube, topSide, 1, rotations);
                        DoMoves(cube, s1, -1, rotations);
                        DoMoves(cube, topSide, 1, rotations);
                        DoMoves(cube, s1, 1, rotations);
                    }

                    break;
                default:
                    throw new Exception("Bad cube");
            }

            sol.rotation = rotations.ToArray();
            solutions.Add(sol);

            return cube;
        }

        private Cube SolveStage3(Cube cube, List<CubeSolutionGroup> solutions)
        {
            cube = (Cube)cube.Clone();
            int topSide = (BaseSide + 3) % 6;
            int f = 0;
            if (cube[topSide, 1] == topSide) f |= 1;
            if (cube[topSide, 3] == topSide) f |= 2;
            if (cube[topSide, 5] == topSide) f |= 4;
            if (cube[topSide, 7] == topSide) f |= 8;

            CubeSolutionGroup sol = new CubeSolutionGroup();
            sol.stage = 3;
            sol.StartCube = (Cube)cube.Clone();

            CubeNavigator nav = new CubeNavigator(cube, topSide, 0);

            List<int> rotations = new List<int>();
            int s1, s2;

            switch (f)
            {
                case 0:
                    nav.MoveNextSide();
                    s1 = nav.Side;
                    nav.MoveNextSide();
                    s2 = nav.Side;
                    sol.frontSide = s2;
                    DoMoves(cube, s2, 1, rotations);
                    DoMoves(cube, s1, 1, rotations);
                    DoMoves(cube, topSide, 1, rotations);
                    DoMoves(cube, s1, -1, rotations);
                    DoMoves(cube, topSide, -1, rotations);
                    DoMoves(cube, s2, -1, rotations);

                    DoMoves(cube, (s2 + 3) % 6, 1, rotations);
                    DoMoves(cube, topSide, 1, rotations);
                    DoMoves(cube, (s1 + 3) % 6, 1, rotations);
                    DoMoves(cube, topSide, -1, rotations);
                    DoMoves(cube, (s1 + 3) % 6, -1, rotations);
                    DoMoves(cube, (s2 + 3) % 6, -1, rotations);
                    break;
                case 3:
                    nav.MoveBy(-2);
                    goto case 6;
                case 6:
                    nav.MoveNextSide();
                    s1 = nav.Side;
                    nav.MoveNextSide();
                    s2 = nav.Side;
                    sol.frontSide = s2;

                    DoMoves(cube, s2, 1, rotations);
                    DoMoves(cube, topSide, 1, rotations);
                    DoMoves(cube, s1, 1, rotations);
                    DoMoves(cube, topSide, -1, rotations);
                    DoMoves(cube, s1, -1, rotations);
                    DoMoves(cube, s2, -1, rotations);

                    break;
                case 12:
                    nav.MoveBy(2);
                    goto case 6;
                case 9:
                    nav.MoveBy(4);
                    goto case 6;
                case 10:
                    nav.MoveNextSide();
                    s1 = nav.Side;
                    nav.MoveNextSide();
                    s2 = nav.Side;
                    sol.frontSide = s2;
                    DoMoves(cube, s2, 1, rotations);
                    DoMoves(cube, s1, 1, rotations);
                    DoMoves(cube, topSide, 1, rotations);
                    DoMoves(cube, s1, -1, rotations);
                    DoMoves(cube, topSide, -1, rotations);
                    DoMoves(cube, s2, -1, rotations);
                    break;
                case 5:
                    nav.MoveBy(2);
                    goto case 10;
                default:
                    throw new Exception("Bad cube");
            }

            sol.rotation = rotations.ToArray();
            solutions.Add(sol);

            return cube;
        }

        private Cube SolveStage2(Cube cube, List<CubeSolutionGroup> solutions)
        {
            int topSide = (BaseSide + 3) % 6;
            bool checkTop = true;
            cube = (Cube)cube.Clone();
            int k = 0;
            while (checkTop)
            {
                if (++k > 20) throw new Exception("Bad algorithm");
                checkTop = false;
                for (int i = 1; i < 8; i += 2)
                {
                    CubeNavigator nav = new CubeNavigator(cube, topSide, i);
                    int s1 = nav.Value;
                    nav.MoveNextSide();
                    int s2 = nav.Value;

                    if (s1 != topSide && s2 != topSide)
                    {
                        List<int> rotations = new List<int>();
                        CubeSolutionGroup sol = new CubeSolutionGroup();
                        sol.StartCube = (Cube)cube.Clone();
                        sol.stage = 2;
                        int t = GetMoves(nav.Side, s2, topSide);
                        DoMoves(cube, topSide, t, rotations);
                        nav.Move(true, t * 2, true);
                        CubeNavigator nav1 = nav;
                        nav1.Move(2, true);
                        if (nav1.Side == s1)
                        {
                            sol.frontSide = s2;
                            DoMoves(cube, topSide, 1, rotations);
                            DoMoves(cube, s1, 1, rotations);
                            DoMoves(cube, topSide, -1, rotations);
                            DoMoves(cube, s1, -1, rotations);
                            DoMoves(cube, topSide, -1, rotations);
                            DoMoves(cube, s2, -1, rotations);
                            DoMoves(cube, topSide, 1, rotations);
                            DoMoves(cube, s2, 1, rotations);
                        }
                        else
                        {
                            sol.frontSide = s1;
                            DoMoves(cube, topSide, -1, rotations);
                            DoMoves(cube, s1, -1, rotations);
                            DoMoves(cube, topSide, 1, rotations);
                            DoMoves(cube, s1, 1, rotations);
                            DoMoves(cube, topSide, 1, rotations);
                            DoMoves(cube, s2, 1, rotations);
                            DoMoves(cube, topSide, -1, rotations);
                            DoMoves(cube, s2, -1, rotations);
                        }

                        sol.rotation = rotations.ToArray();
                        solutions.Add(sol);
                        checkTop = true;
                    }
                }

                if (!checkTop)
                {
                    CubeNavigator nav = new CubeNavigator(cube, topSide, 1);
                    nav.Move(true, 2);
                    for (int i = 0; i < 4; i++)
                    {
                        int s1 = nav.Side;
                        int c1 = nav.Value;
                        nav.MoveNextSide();
                        int s2 = nav.Side;
                        int c2 = nav.Value;
                        if (s1 != c1 || s2 != c2)
                        {
                            List<int> rotations = new List<int>();
                            CubeSolutionGroup sol = new CubeSolutionGroup();
                            sol.StartCube = (Cube)cube.Clone();
                            sol.stage = 2;
                            sol.frontSide = s1;

                            if (s1 == c2 && s2 == c1)
                            {
                                DoMoves(cube, s2, 1, rotations);
                                DoMoves(cube, topSide, -1, rotations);
                                DoMoves(cube, s2, -1, rotations);
                                DoMoves(cube, topSide, -1, rotations);
                                DoMoves(cube, s1, -1, rotations);
                                DoMoves(cube, topSide, 1, rotations);
                                DoMoves(cube, s1, 1, rotations);

                                DoMoves(cube, topSide, -1, rotations);
                                DoMoves(cube, s2, 1, rotations);
                                DoMoves(cube, topSide, -1, rotations);
                                DoMoves(cube, s2, -1, rotations);
                                DoMoves(cube, topSide, -1, rotations);
                                DoMoves(cube, s1, -1, rotations);
                                DoMoves(cube, topSide, 1, rotations);
                                DoMoves(cube, s1, 1, rotations);
                            }
                            else
                            {
                                DoMoves(cube, s2, 1, rotations);
                                DoMoves(cube, topSide, -1, rotations);
                                DoMoves(cube, s2, -1, rotations);
                                DoMoves(cube, topSide, -1, rotations);
                                DoMoves(cube, s1, -1, rotations);
                                DoMoves(cube, topSide, 1, rotations);
                                DoMoves(cube, s1, 1, rotations);
                                checkTop = true;
                            }

                            sol.rotation = rotations.ToArray();
                            solutions.Add(sol);
                        }
                        nav.MoveBy(4);
                    }
                }
            }

            return cube;
        }

        private Cube SolveStage1(Cube cube, List<CubeSolutionGroup> solutions)
        {
            int topSide = (BaseSide + 3) % 6;

            cube = (Cube)cube.Clone();
            for (int i = 0; i < 7; i += 2)
            {
                CubeNavigator nav = new CubeNavigator(cube, BaseSide, i);
                int c1, c2;
                int s1, s2;
                nav.MoveNextSide();
                c1 = nav.Value;
                s1 = nav.Side;
                nav.MoveNextSide();
                c2 = nav.Value;
                s2 = nav.Side;
                nav.MoveNextSide();

                if (nav.Value != BaseSide || c1 != s1 || c2 != s2)
                {
                    List<int> rotations = new List<int>();
                    CubeSolutionGroup sol = new CubeSolutionGroup();
                    sol.frontSide = s1;
                    sol.StartCube = (Cube)cube.Clone();
                    sol.stage = 1;

                    for (int j = 0; j < 6; j++)
                    {
                        for (int q = 0; q < 7; q += 2)
                        {
                            if (cube[j, q] == BaseSide)
                            {
                                bool isOnBaseSide = false;
                                CubeNavigator nav2 = new CubeNavigator(cube, j, q);
                                nav2.MoveNextSide();
                                bool found = nav2.Value == s1;
                                if (nav2.Side == BaseSide) isOnBaseSide = true;
                                nav2.MoveNextSide();
                                found = found && nav2.Value == s2;
                                if (nav2.Side == BaseSide) isOnBaseSide = true;
                                if (found)
                                {
                                    if (j == BaseSide || isOnBaseSide)
                                    {
                                        if (j == BaseSide)
                                        {
                                            // check it
                                            DoMoves(cube, nav2.Side, 1, rotations);
                                            DoMoves(cube, topSide, 1, rotations);
                                            DoMoves(cube, nav2.Side, -1, rotations);
                                            nav2.Move(false, 4);
                                        }
                                        else if (nav2.Side == BaseSide)
                                        {
                                            DoMoves(cube, j, -1, rotations);
                                            DoMoves(cube, topSide, -1, rotations);
                                            DoMoves(cube, j, 1, rotations);
                                            nav2.Move(false, 4, false);
                                        }
                                        else
                                        {
                                            DoMoves(cube, j, 1, rotations);
                                            DoMoves(cube, topSide, 1, rotations);
                                            DoMoves(cube, j, -1, rotations);
                                            nav2.Move(4, false);
                                        }

                                    }
                                    else if (j == topSide)
                                    {
                                        int t = GetMoves(nav2.Side, s1, topSide);
                                        DoMoves(cube, topSide, t, rotations);
                                        nav2.Move(true, t * 2, true);
                                        DoMoves(cube, nav2.Side, 1, rotations);
                                        DoMoves(cube, topSide, -1, rotations);
                                        DoMoves(cube, nav2.Side, -1, rotations);
                                        nav2.Move(false, 4);
                                    }

                                    if (nav2.Side == topSide)
                                    {
                                        nav2.MovePrevSide();
                                        int t = GetMoves(nav2.Side, s1, topSide);
                                        DoMoves(cube, topSide, t, rotations);

                                        DoMoves(cube, s2, 1, rotations);
                                        DoMoves(cube, topSide, 1, rotations);
                                        DoMoves(cube, s2, -1, rotations);
                                    }
                                    else
                                    {
                                        int t = GetMoves(nav2.Side, s2, topSide);
                                        DoMoves(cube, topSide, t, rotations);

                                        DoMoves(cube, s1, -1, rotations);
                                        DoMoves(cube, topSide, -1, rotations);
                                        DoMoves(cube, s1, 1, rotations);
                                    }
                                    goto corner_in_place;
                                }
                            }
                        }
                    }
                    throw new Exception("Bad cube (2)");
                corner_in_place: ;

                    sol.rotation = rotations.ToArray();
                    solutions.Add(sol);
                }
            }
            return cube;
        }

        private Cube SolveStage0(Cube cube, List<CubeSolutionGroup> solutions)
        {
            cube = (Cube)cube.Clone();

            CubeSolutionGroup sol = new CubeSolutionGroup();
            sol.stage = 0;
            sol.StartCube = (Cube)cube.Clone();

            List<int> rotations = new List<int>();

            if (cube[BaseSide, 1] != BaseSide && cube[BaseSide, 3] != BaseSide &&
                cube[BaseSide, 5] != BaseSide && cube[BaseSide, 7] != BaseSide)
            {
                bool onParallel = false;
                CubeNavigator nav = new CubeNavigator(cube, BaseSide, 1);
                nav.MoveNextSide();
                nav.MoveBy(2);
                for (int i = 0; i < 4; i++)
                {
                    if (nav.Value == BaseSide) { onParallel = true; break; }
                    nav.MoveBy(4);
                    if (nav.Value == BaseSide) { onParallel = true; break; }
                    nav.MoveNextSide();
                }

                bool specialMove = false;
                if (!onParallel)
                {
                    nav.MoveBy(-2);
                    for (int i = 0; i < 4; i++)
                    {
                        if (nav.Value == BaseSide) break;
                        nav.MoveBy(4);

                        if (nav.Value == BaseSide) break;
                        nav.MoveBy(1);
                        nav.MovePrevSide();
                        nav.MoveBy(1);
                    }

                    if (nav.Value == BaseSide)
                    {
                        DoMoves(cube, nav.Side, 1, rotations);
                        nav.MoveBy(2);
                    }
                    else
                    {
                        specialMove = true;
                        DoMoves(cube, nav.Side, 2, rotations);
                    }
                }

                if (!specialMove)
                {
                    CubeNavigator nav2 = nav;
                    nav2.MoveBy(2);
                    nav2.MoveNextSide();
                    nav.MoveNextSide();
                    if (nav2.Side == BaseSide)
                        DoMoves(cube, nav.Side, -1, rotations);
                    else
                        DoMoves(cube, nav.Side, 1, rotations);
                }
            }


            int frontSide = -1;
            int frontFoundAt = -1;
            CubeNavigator nav0 = new CubeNavigator(cube, BaseSide, 1);
            for (int i = 0; i < 4; i++)
            {
                if (nav0.Value == BaseSide)
                {
                    CubeNavigator nav = nav0;
                    nav.MoveNextSide();
                    if (frontSide < 0)
                    {
                        frontSide = nav.Value;
                        frontFoundAt = nav.Side;
                        if (frontSide == frontFoundAt) break;
                    }
                    else if (nav.Value == nav.Side)
                    {
                        frontSide = frontFoundAt = nav.Value;
                        break;
                    }
                }
                nav0.MoveBy(2);
            }

            if (frontSide < 0) throw new Exception("Bad cube");



            int t = GetMoves(frontFoundAt, frontSide, BaseSide);
            DoMoves(cube, BaseSide, t, rotations);

            sol.frontSide = frontSide;
            sol.rotation = rotations.ToArray();
            solutions.Add(sol);

            int topSide = (BaseSide + 3) % 6;

            for (int i = 1; i < 8; i += 2)
            {

                CubeNavigator nav = new CubeNavigator(cube, BaseSide, i);
                nav.MoveNextSide();
                if (nav.Value != nav.Side || cube[BaseSide, i] != BaseSide)
                {
                    rotations.Clear();
                    sol = new CubeSolutionGroup();
                    sol.frontSide = frontSide;
                    sol.stage = 0;
                    sol.StartCube = (Cube)cube.Clone();

                    int currentSide = nav.Side;
                    CubeNavigator nav1;
                    int lastSide = currentSide;
                    for (int j = 0; j < 6; j++)
                    {
                        for (int q = 1; q < 8; q += 2)
                        {
                            nav1 = new CubeNavigator(cube, j, q);
                            if (nav1.Value == currentSide)
                            {
                                nav1.MoveNextSide();
                                if (nav1.Value == BaseSide)
                                {
                                    if (nav1.Side == BaseSide)
                                    {
                                        DoMoves(cube, j, 1, rotations);
                                        nav1.Move(true, 2);
                                    }
                                    else if (nav1.Side == topSide)
                                    {
                                        int t2 = GetMoves(currentSide, j, BaseSide);
                                        DoMoves(cube, BaseSide, t2, rotations);
                                        lastSide = j;
                                        DoMoves(cube, j, 1, rotations);
                                        nav1.Move(true, 2);
                                    }
                                    else if (j == BaseSide)
                                    {
                                        DoMoves(cube, nav1.Side, 1, rotations);
                                        nav1.Move(2, true);
                                    }
                                    else if (j == topSide)
                                    {
                                        int t2 = GetMoves(currentSide, nav1.Side, BaseSide);
                                        DoMoves(cube, BaseSide, t2, rotations);
                                        lastSide = nav1.Side;
                                        DoMoves(cube, nav1.Side, 1, rotations);
                                        nav1.Move(2, true);
                                    }
                                    else
                                    {
                                        nav1.MoveNextSide();
                                    }
                                    goto edge_piece_found;
                                }
                            }
                        }
                    }
                    throw new Exception("Bad cube");
                edge_piece_found: ;
                    DoMoves(cube, BaseSide, GetMoves(lastSide, nav1.Side, BaseSide), rotations);
                    CubeNavigator nav2 = nav1;
                    nav2.Move(2, true);
                    if (nav2.Side == BaseSide)
                        DoMoves(cube, nav1.Side, 1, rotations);
                    else
                        DoMoves(cube, nav1.Side, -1, rotations);
                    DoMoves(cube, BaseSide, GetMoves(nav1.Side, currentSide, BaseSide), rotations);

                    sol.rotation = rotations.ToArray();
                    solutions.Add(sol);
                }
            }
            return cube;
        }

        public static void DoMoves(Cube cube, int side, int count, List<int> rotations)
        {
            switch (count & 3)
            {
                case 1:
                    rotations.Add(side);
                    cube.RotateSide(side, false);
                    break;
                case 2:
                    rotations.Add(side);
                    cube.RotateSide(side, false);
                    rotations.Add(side);
                    cube.RotateSide(side, false);
                    break;
                case 3:
                    rotations.Add(~side);
                    cube.RotateSide(side, true);
                    break;
            }
        }

        public static int GetMoves(int from, int to, int BaseSide)
        {
            if (from != to)
            {
                int i = -1;
                int j = -1;
                for (int q = 0; q < 4; q++)
                {
                    if (Cube.RotationNeighbors[BaseSide, q] == to) i = q;
                    if (Cube.RotationNeighbors[BaseSide, q] == from) j = q;
                }
                if (i < j) i += 4;
                switch (i - j)
                {
                    case 1:
                        return 1;
                    case 2:
                        return 2;
                    case 3:
                        return -1;
                }
            }
            return 0;
        }

        public int GetCubeStage(Cube cube)
        {
            int stage = 0;
            if (cube[BaseSide, 1] == BaseSide && cube[BaseSide, 3] == BaseSide &&
                cube[BaseSide, 5] == BaseSide && cube[BaseSide, 7] == BaseSide)
            {
                bool valid = true;
                CubeNavigator nav = new CubeNavigator(cube, BaseSide, 1);
                nav.MoveNextSide();
                for (int i = 0; i < 4; i++)
                {
                    if (nav.Value != nav.Side) valid = false;
                    nav.Move(true, 2, true);
                }
                if (valid) stage = 1;
            }

            if (stage == 1 && cube[BaseSide, 0] == BaseSide && cube[BaseSide, 2] == BaseSide &&
                cube[BaseSide, 4] == BaseSide && cube[BaseSide, 6] == BaseSide)
            {
                bool valid = true;
                CubeNavigator nav = new CubeNavigator(cube, BaseSide, 0);
                nav.MovePrevSide();
                for (int i = 0; i < 4; i++)
                {
                    if (nav.Value != nav.Side) valid = false;
                    nav.MoveBy(-2);
                    if (nav.Value != nav.Side) valid = false;
                    nav.MoveNextSide();
                }
                if (valid) stage = 2;
            }

            if (stage == 2)
            {
                bool valid = true;
                CubeNavigator nav = new CubeNavigator(cube, BaseSide, 1);
                nav.MoveNextSide();
                nav.MoveBy(2);
                for (int i = 0; i < 4; i++)
                {
                    if (nav.Value != nav.Side) valid = false;
                    nav.MoveBy(4);
                    if (nav.Value != nav.Side) valid = false;
                    nav.MoveNextSide();
                }
                if (valid) stage = 3;
            }

            int topSide = (BaseSide + 3) % 6;

            if (stage == 3 && cube[topSide, 1] == topSide && cube[topSide, 3] == topSide &&
                cube[topSide, 5] == topSide && cube[topSide, 7] == topSide)
            {
                stage = 4;
            }

            if (stage == 4 && cube[topSide, 0] == topSide && cube[topSide, 2] == topSide &&
                cube[topSide, 4] == topSide && cube[topSide, 6] == topSide)
            {
                stage = 5;
            }

            if (stage == 5)
            {
                bool valid = true;
                CubeNavigator nav = new CubeNavigator(cube, topSide, 0);
                nav.MoveNextSide();
                for (int i = 0; i < 4; i++)
                {
                    if (nav.Value != nav.Side) valid = false;
                    nav.MoveBy(2);
                    if (nav.Value != nav.Side) valid = false;
                    nav.MovePrevSide();
                }
                if (valid) stage = 6;
            }

            if (stage == 6)
            {
                bool valid = true;
                CubeNavigator nav = new CubeNavigator(cube, topSide, 1);
                nav.MoveNextSide();
                for (int i = 0; i < 4; i++)
                {
                    if (nav.Value != nav.Side) valid = false;
                    nav.Move(true, 2, true);
                }
                if (valid) stage = 7;
            }

            return stage;
        }
    }
}
