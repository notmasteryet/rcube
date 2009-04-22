using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RCube
{
    public partial class CaptureSideEditForm : Form
    {
        int[] sideColors;

        public int[] SideColors
        {
            get { return sideColors; }
            set { sideColors = value; }
        }

        Color[] colors;

        public Color[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        public CaptureSideEditForm()
        {
            InitializeComponent();
        }


        private void sidePictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int q = i * 3 + j;

                    e.Graphics.FillRectangle(new SolidBrush(Colors[SideColors[q]]),
                        j * 53 + 1, i * 53 + 1, 52, 52);
                }
            }
        }

        private void CaptureSideEditForm_Deactivate(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}