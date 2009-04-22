using System;
using System.Collections.Generic;
using System.Drawing;

namespace RCube
{
    static class CubeSidesColorOffset
    {
        public static Color SuggestedBottomColor = Color.Green;
        public static Color SuggestedFrontColor = Color.White;

        public static int[] FindTransform(Color[] colors)
        {
            if (colors == null || colors.Length != 6) throw new ArgumentException("colors");

            int[] transform = null;

            int bottomIndex = IndexOfCloseOne(colors, SuggestedBottomColor);
            switch (bottomIndex)
            {
                case 0:
                    transform = new int[] { 3, 5, 4, 0, 2, 1 };
                    break;
                case 1:
                    transform = new int[] { 1, 3, 2, 4, 0, 5 };
                    break;
                case 2:
                    transform = new int[] { 2, 1, 3, 5, 4, 0 };
                    break;
                case 3:
                    transform = new int[] { 0, 1, 2, 3, 4, 5 };
                    break;
                case 4:
                    transform = new int[] { 4, 0, 2, 1, 3, 5 };
                    break;
                case 5:
                    transform = new int[] { 5, 1, 0, 2, 4, 3 };
                    break;
            }

            Color[] currentColors = new Color[6];
            for (int i = 0; i < colors.Length; i++)
            {
                currentColors[transform[i]] = colors[i];
            }

            int frontIndex = IndexOfCloseOne(new Color[] { 
                currentColors[1], currentColors[2], currentColors[4], currentColors[5]},
                SuggestedFrontColor);

            if (frontIndex < 2) frontIndex += 1; else frontIndex += 2;

            int[] transform2 = null;
            switch (frontIndex)
            {
                case 2:
                    transform2 = new int[] { 0, 2, 4, 3, 5, 1 };
                    break;
                case 4:
                    transform2 = new int[] { 0, 4, 5, 3, 1, 2 };
                    break;
                case 5:
                    transform2 = new int[] { 0, 5, 1, 3, 2, 4 };
                    break;

            }

            if (transform2 != null)
            {
                int[] newTransform = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    newTransform[i] = transform2[transform[i]];
                }
                transform = newTransform;
            }

            return transform;
        }

        private static int IndexOfCloseOne(Color[] colors, Color c)
        {
            int minDistance = int.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < colors.Length; i++)
            {
                int distance = ColorDistance(colors[i], c);
                if (minDistance > distance)
                {
                    minIndex = i;
                    minDistance = distance;
                }
            }
            return minIndex;
        }

        private static int ColorDistance(Color c1, Color c2)
        {
            int dr = c1.R - c2.R, dg = c1.G - c2.G, db = c1.B - c2.B;
            return dr * dr + dg * dg + db * db;
        }
    }
}
