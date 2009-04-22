using System;
using System.Collections.Generic;
using System.Text;

using OpenCVProxy;
using OpenCVProxy.Interop;

namespace RCubeCapture
{
    public static class ColorGroups
    {
        /// <summary>
        /// Autodetects colors of the groups
        /// </summary>
        /// <param name="sides">List of sides (side number by colors)</param>
        /// <param name="colors">Mean of color in the group (group by color)</param>
        /// <param name="nearestColorIndex">Color indexes of the side items (side by item)</param>
        public static void GetColorGroups(IList<int[,]> sides, out int[,] colors, out int[,] nearestColorIndex)
        {
            CvMatSingle samples = new CvMatSingle(sides.Count * 9, 1, 3);
            for (int i = 0; i < sides.Count; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    samples[i * 9 + j, 0, 0] = sides[i][j, 0];
                    samples[i * 9 + j, 0, 1] = sides[i][j, 1];
                    samples[i * 9 + j, 0, 2] = sides[i][j, 2];
                }
            }

            CvMatInt32 groups = new CvMatInt32(sides.Count * 9, 1, 1);

            CxCore.cvKMeans2(samples, 6, groups, new CvTermCriteria(10, 1.0));

            nearestColorIndex = new int[sides.Count, 9];
            ColorStatData[] groupColors = new ColorStatData[6];
            for (int i = 0; i < sides.Count * 9; i++)
            {
                int group = groups[i, 0];
                groupColors[group].AddSamples(sides[i / 9][i % 9, 0],
                    sides[i / 9][i % 9, 1], sides[i / 9][i % 9, 2]);
                nearestColorIndex[i / 9, i % 9] = group;
            }

            colors = new int[6, 3];
            for (int i = 0; i < 6; i++)
            {
                colors[i, 0] = (int)(groupColors[i].r / groupColors[i].count);
                colors[i, 1] = (int)(groupColors[i].g / groupColors[i].count);
                colors[i, 2] = (int)(groupColors[i].b / groupColors[i].count);
            }     
        }

        private struct ColorStatData
        {
            public int r;
            public int g;
            public int b;
            public int count;

            public void AddSamples(int r, int g, int b)
            {
                this.r += r;
                this.g += g;
                this.b += b;
                this.count++;
            }
        }

    }
}
