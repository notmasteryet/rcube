using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RCube
{
    public partial class ChangePaletteForm : Form
    {
        public static Color[] DefaultPalette = { Color.Blue, Color.White, Color.FromArgb(224, 112, 0), Color.Green, Color.Yellow, Color.FromArgb(160, 0, 0)};

        static int[, ,] PreviewLayout = {
            {{0, 0}, {4, 2}, {0, 4}, {-4, 2}},
            {{0, 0}, {0, -5}, {4, -3}, {4, 2}},
            {{0, 0}, {-4, 2}, {-4, -3}, {0, -5}}
        };

        private Color[] palette = DefaultPalette;
        private Point[][] drawnSides = null;

        public Color[] Palette
        {
            get { return palette; }
            set { palette = value; }
        }

        public ChangePaletteForm()
        {
            InitializeComponent();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            Palette = DefaultPalette;
            previewPictureBox.Invalidate();
        }

        private void previewPictureBox_Paint(object sender, PaintEventArgs e)
        {
            // build sides
            drawnSides = new Point[6][];

            int unitSize = Math.Min(previewPictureBox.Width / 17, previewPictureBox.Height / 9);
            // visible part
            for (int i = 0; i < 3; i++)
            {
                Point[] side = new Point[5];
                for (int j = 0; j < 4; j++)
                {
                    side[j] = new Point(unitSize * (PreviewLayout[i, j, 0] + 4),
                        unitSize * (4 - PreviewLayout[i, j, 1]));
                }
                side[4] = side[0];
                drawnSides[i] = side;
            }
            // invisible part
            for (int i = 0; i < 3; i++)
            {
                Point[] side = new Point[5];
                for (int j = 0; j < 4; j++)
                {
                    side[j] = new Point(unitSize * (13 + PreviewLayout[i, j, 0]),
                        unitSize * (PreviewLayout[i, j, 1] + 5));
                }
                side[4] = side[0];
                drawnSides[i + 3] = side;
            }

            for (int i = 0; i < 6; i++)
            {
                e.Graphics.FillPolygon(new SolidBrush(Palette[i]),
                    drawnSides[i], System.Drawing.Drawing2D.FillMode.Winding);
            }
        }

        private void ChangeColor(int index)
        {
            colorChangeDialog.Color = Palette[index];
            if (colorChangeDialog.ShowDialog() == DialogResult.OK)
            {
                Color[] newColors = (Color[])Palette.Clone();
                newColors[index] = colorChangeDialog.Color;
                Palette = newColors;
                previewPictureBox.Invalidate();
            }
        }

        private void previewPictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                // check if side was clicked
                bool isInside = true;
                int sign = 0;
                for (int j = 0; j < 4; j++)
                {
                    int test = (e.Y - drawnSides[i][j].Y) * (drawnSides[i][j + 1].X - drawnSides[i][j].X)
                        - (e.X - drawnSides[i][j].X) * (drawnSides[i][j + 1].Y - drawnSides[i][j].Y);

                    if ((sign < 0 && test > 0) || (sign > 0 && test < 0))
                        isInside = false;
                    else if (sign == 0)
                        sign = test;
                }

                if (isInside)
                {
                    ChangeColor(i);
                    break;
                }
            }
        }

        private void topButton_Click(object sender, EventArgs e)
        {
            ChangeColor(0);
        }

        private void leftButton_Click(object sender, EventArgs e)
        {
            ChangeColor(2);
        }

        private void frontButton_Click(object sender, EventArgs e)
        {
            ChangeColor(1);
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            ChangeColor(5);
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            ChangeColor(4);
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            ChangeColor(3);
        }

    }
}