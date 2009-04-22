using System;
using System.Collections.Generic;
using System.Text;

namespace RCube
{
    class CubeSolverFactory
    {
        public CubeSolver CreateCubeSolver()
        {
            return new KnightsCubeSolver();
        }
    }
}
