using System;
using System.Collections.Generic;
using System.Text;

namespace RCube
{
    /// <summary>
    /// Abstract class for cube solving.
    /// </summary>
    abstract class CubeSolver
    {
        private int baseSide = 3;

        /// <summary>
        /// Gets or sets base (bottom) side number.
        /// </summary>
        public int BaseSide
        {
            get { return baseSide; }
            set { baseSide = value; }
        }

        /// <summary>
        /// Solves the cube.
        /// </summary>
        /// <param name="cube">Cube</param>
        /// <returns>Solution groups</returns>
        public abstract CubeSolutionGroup[] Solve(Cube cube);
    }

    class CubeSolutionGroup
    {
        public Cube StartCube;
        public int stage;
        public int[] rotation;
        public int frontSide;
    }
}
