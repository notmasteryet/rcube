using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Rubik_s_Cube.WebCamera;

namespace Rubik_s_Cube
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Image img = null;

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ReplaceImage(Image.FromFile(openFileDialog1.FileName) as Bitmap);

                this.Text = System.IO.Path.GetFileName(openFileDialog1.FileName);
            }
        }

        private void ReplaceImage(Bitmap newImage)
        {
            if (img != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
                img.Dispose();
                img = null;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                img = newImage;
                InitializeColors(newImage);

                imageWidth = img.Width;
                imageHeight = img.Height;

                pictureBox1.Image = newImage;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void InitializeColors(Bitmap bmp)
        {
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
               System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            colors = new HSVColor[bmp.Width, bmp.Height];
            unsafe
            {
                byte* p = (byte*)data.Scan0.ToPointer();
                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        byte r = *(p++);
                        byte g = *(p++);
                        byte b = *(p++);
                        colors[j,i] = HSVColor.FromRGB(r, g, b);
                    }
                }
            }
            bmp.UnlockBits(data);
        }

        private int imageWidth;
        private int imageHeight;
        private HSVColor[,] colors;
        
        private Bitmap black = null;
        private Point[][] rects = null;

        private const bool SHOW_BLACK = true;

        private void button1_Click(object sender, EventArgs e)
        {
            FindEdge();
        }

        private void FindEdge()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                bool[,] black2 = FindCube.FindEdge(colors);

                if (checkBox1.Checked)
                {
                    Bitmap res;
                    BuildBlackBitmap(black2, out res);

                    black = res;
                }
                else
                {
                    black = null;
                }

                int[][,] r0 = FindCube.FindSides(black2);
                Point[][] rects = new Point[r0.Length][];
                for (int i = 0; i < r0.Length; i++)
                {
                    Point[] rect = new Point[4];
                    for (int j = 0; j < 4; j++)
                    {
                        rect[j] = new Point(r0[i][j, 1], r0[i][j, 0]);
                    }
                    rects[i] = rect;
                }
                this.rects = rects;

                // PotentialCube cubes = FindCube.FindPotencialCubes(r0);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
            pictureBox1.Refresh();
        }

        private void BuildBlackBitmap(bool[,] black2, out Bitmap res)
        {
            res = new Bitmap(imageWidth, imageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int i = 0; i < imageHeight; i++)
            {
                char[] line = new char[imageWidth];
                for (int j = 0; j < imageWidth; j++)
                {
                    if (black2[i, j])
                    {
                        res.SetPixel(j, i, Color.Red);
                    }
                    else
                    {
                        // res.SetPixel(j, i, Color.Transparent);
                    }
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (black != null)
                e.Graphics.DrawImage(black, 0, 0);
            if (rects != null)
            {
                foreach(Point[] p in rects)
                {
                    e.Graphics.DrawPolygon(Pens.Green, p);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ReplaceImage(pictureBox2.Image.Clone() as Bitmap);

            this.Text = "Camera @ " + DateTime.Now.ToShortTimeString();
        }

        private WebCamera.WebCameraSupport webCamera = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = (int)numericUpDown1.Value * 1000;
            try
            {
                webCamera = new WebCamera.WebCameraSupport(Properties.Settings.Default.WebCameraName);
                webCamera.Start();
                webCamera.NewFrame += new Rubik_s_Cube.WebCamera.CameraEventHandler(webCamera_NewFrame);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                webCamera = null;
            }

        }

        void webCamera_NewFrame(object sender, Rubik_s_Cube.WebCamera.CameraEventArgs e)
        {
            Image oldImage = pictureBox2.Image;
            pictureBox2.Image = e.Bitmap.Clone() as Image;
            if (oldImage != null) oldImage.Dispose();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (webCamera != null)
            {
                webCamera.NewFrame -= new CameraEventHandler(webCamera_NewFrame);
                webCamera.Stop();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = (int)numericUpDown1.Value * 1000;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox2.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReplaceImage(pictureBox2.Image.Clone() as Bitmap);

            this.Text = "Camera @ " + DateTime.Now.ToShortTimeString();

            FindEdge();
        }


    }

}