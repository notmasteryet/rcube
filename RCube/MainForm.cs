using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RCube
{
    public partial class MainForm : Form
    {
        private static int Distance = 50;
        private static Color EmptySideColor = Color.Gray;
        private static string[] SideNames = { "Top", "Front", "Left", "Down", "Back", "Right" };
        private static Matrix UpPerspectiveRotation;
        private static Matrix DownPerspectiveRotation;
        private static string RotationChars = "urfdlb";
        private static double[] cdfTable = { 0, 0.05, 0.1, 0.2, 0.33, 0.5, 0.66, 0.8, 0.9, 0.95, 1.0 };

        private enum RotationCommand : int
        {
            None = -1,
            Up = 0,
            Right,
            Front,
            Down,
            Left,
            Back
        }

        static MainForm()
        {
            double pi2 = Math.PI / 2;
            double pi4 = Math.PI / 4;
            UpPerspectiveRotation = Transform3D.GetXRotation(-pi4) *
                Transform3D.GetYRotation(Math.PI + pi4) * Transform3D.GetXRotation(pi2);
            DownPerspectiveRotation = Transform3D.GetXRotation(pi4) *
                Transform3D.GetYRotation(Math.PI + pi4) * Transform3D.GetXRotation(pi2);
        }

        private int[,] colors = new int[6, 9];
        private int[,] colorsEntered;

        private Color[] currentPalette = ChangePaletteForm.DefaultPalette;

        public Color[] CurrentPalette
        {
            get { return currentPalette; }
        }

        private int selectedColor = 0;

        private Matrix rotation = Transform3D.NoRotation;

        private ScreenCubeDraw draw = null;
        private PieceBounds[] pieces = null;
        private int? side = null;
        private double sideAngle = 0;
        private int lastX;
        private int lastY;
        private bool isMoving = false;
        private bool cancelClick = false;
        private SolutionViewForm solutionView = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private Matrix GetTransformFor(int frontSide, bool downPerspective)
        {
            double pi2 = Math.PI / 2;
            double pi4 = Math.PI / 4;
            double angle = GetRotationAngleForFront(frontSide);
            return Transform3D.GetXRotation(downPerspective ? pi4 : -pi4) *
                            Transform3D.GetYRotation(angle) * Transform3D.GetXRotation(pi2);
        }

        private Matrix GetTransformForRotation(int fromSide, int toSide, double position, bool downPerspective)
        {
            double pi2 = Math.PI / 2;
            double pi4 = Math.PI / 4;
            double angle = GetRotationAngleForFront(fromSide);
            double angle2 = GetRotationAngleForFront(toSide);
            // assert: max difference 1.5 * PI
            if (Math.Abs(angle2 - angle) > Math.PI)
            {
                if (angle2 < angle) angle2 += 2 * Math.PI; else angle += 2 * Math.PI;
            }

            return Transform3D.GetXRotation(downPerspective ? pi4 : -pi4) *
                            Transform3D.GetYRotation(angle + (angle2 - angle) * position) * Transform3D.GetXRotation(pi2);
        }

        private double GetRotationAngleForFront(int frontSide)
        {
            double pi4 = Math.PI / 4;
            switch (frontSide)
            {
                case 1:
                    return Math.PI + pi4;
                case 2:
                    return Math.PI - pi4;
                case 4:
                    return pi4;
                case 5:
                    return -pi4;
                default:
                    return 0;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (side.HasValue)
                draw.DrawRotation(e.Graphics, new RectangleF(0, 0, cube3DPictureBox.Width, cube3DPictureBox.Height), rotation, side.Value, sideAngle);
            else
                pieces = draw.Draw(e.Graphics, new RectangleF(0, 0, cube3DPictureBox.Width, cube3DPictureBox.Height), rotation);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            lastX = e.X;
            lastY = e.Y;
            isMoving = true;
            cancelClick = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                double dX = Math.Atan2(e.X - cube3DPictureBox.Width / 2.0, Distance) - Math.Atan2(lastX - cube3DPictureBox.Width / 2.0, Distance);
                double dY = Math.Atan2(e.Y - cube3DPictureBox.Height / 2.0, Distance) - Math.Atan2(lastY - cube3DPictureBox.Height / 2.0, Distance);
                if (dX != 0)
                {
                    rotation = Transform3D.GetYRotation(-dX) * rotation;
                }
                if (dY != 0)
                {
                    rotation = Transform3D.GetXRotation(-dY) * rotation;
                }
                lastX = e.X;
                lastY = e.Y;
                cube3DPictureBox.Invalidate();
                cancelClick = true;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMoving = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            draw = new ScreenCubeDraw(this);
            if (Properties.Settings.Default.FirstRun)
            {
                Properties.Settings.Default.FirstRun = false;
                Properties.Settings.Default.Save();

                if (MessageBox.Show("Do you want to start demo?\nSame can be done by full cube creation, by mixing it, and by auto-solving.", "First time run", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    fullToolStripMenuItem_Click(this, EventArgs.Empty);
                    UpdateColorToolStrip();

                    mixToolStripMenuItem_Click(this, EventArgs.Empty);
                    solveToolStripMenuItem_Click(this, EventArgs.Empty);
                    return;
                }
            }

            if (Properties.Settings.Default.LastCube != "")
            {
                string[] s = Properties.Settings.Default.LastCube.Split(' ');
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        colors[i, j] = int.Parse(s[i * 9 + j]);
                    }
                }
            }
            colorsEntered = (int[,])colors.Clone();

            UpdateColorToolStrip();
        }

        private void UpdateColorToolStrip()
        {
            int q = 0;
            foreach (ToolStripButton btn in colorToolStrip.Items)
            {
                btn.Image = GetColorButton(CurrentPalette[q]);
                btn.Text = CurrentPalette[q].Name;
                btn.ToolTipText = String.Format("{1} ({0})", CurrentPalette[q].Name, SideNames[q]);
                q++;
            }
        }

        private Bitmap GetColorButton(Color color)
        {
            Bitmap bmp = new Bitmap(24, 24, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillEllipse(new SolidBrush(color), 0, 0, 23, 23);
            }
            return bmp;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (cancelClick) return;

            foreach (PieceBounds piece in pieces)
            {
                if (piece.InPiece(e.X, e.Y) && selectedColor > 0)
                {
                    if (colorsEntered == null) colorsEntered = colors;
                    colorsEntered[piece.Side, piece.Piece] = selectedColor;

                    DoColorsInput();
                    cube3DPictureBox.Invalidate();
                }
            }
        }

        private void DoColorsInput()
        {
            if (autocompleteToolStripMenuItem.Checked)
            {
                colors = (int[,])colorsEntered.Clone();
                try
                {
                    ColorFinder.FindColors(colors);
                }
                catch
                {
                }
            }
            else
                colors = colorsEntered;
        }

        private void Form2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '6')
            {
                SetSelectedColor(e.KeyChar - '0');
                e.Handled = true;
            }
            else
            {
                int i = GetSideOfCommand(e.KeyChar);
                bool reverse = Char.IsUpper(e.KeyChar);
                if (i >= 0 && !moveWorker.IsBusy)
                {
                    moveWorker.RunWorkerAsync((reverse ? ~i : i));
                    e.Handled = true;
                }
            }
        }

        private void DoRotationCommand(char command)
        {
            int i = GetSideOfCommand(command);
            if (i >= 0 && !moveWorker.IsBusy)
            {
                moveWorker.RunWorkerAsync(i);
                colorsEntered = null;
            }
        }

        private int GetSideOfCommand(char command)
        {
            int commandIndex = RotationChars.IndexOf(Char.ToLower(command));
            Dictionary<int, PointF> centers = new Dictionary<int, PointF>();
            for (int i = 0; i < pieces.Length; i++)
            {
                if (pieces[i].Piece == 8)
                {
                    PointF p = new PointF((pieces[i].Bounds[0].X + pieces[i].Bounds[2].X) / 2,
                        (pieces[i].Bounds[0].Y + pieces[i].Bounds[2].Y) / 2);
                    centers.Add(pieces[i].Side, p);
                }
            }
            if (centers.Count != 3) return -1;
            int[] sides = new int[3];
            PointF[] centerPoints = new PointF[3];
            int j = 0;
            foreach (KeyValuePair<int, PointF> item in centers)
            {
                sides[j] = item.Key;
                centerPoints[j] = item.Value;
                j++;
            }

            Array.Sort<PointF, int>(centerPoints, sides, new PointFXComparer());

            if (Math.Abs(Math.Atan2(centerPoints[2].Y - centerPoints[0].Y, centerPoints[2].X - centerPoints[0].X)) < Math.PI / 6)
            {
                bool upVisible = Math.Max(centerPoints[2].Y, centerPoints[0].Y) > centerPoints[1].Y;
                int frontSide = sides[0];
                int rightSide = sides[2];
                int upSide = upVisible ? sides[1] : ((sides[1] + 3) % 6);
                switch ((RotationCommand)commandIndex)
                {
                    case RotationCommand.Front:
                        return frontSide;
                    case RotationCommand.Up:
                        return upSide;
                    case RotationCommand.Right:
                        return rightSide;
                    case RotationCommand.Back:
                        return (frontSide + 3) % 6;
                    case RotationCommand.Down:
                        return (upSide + 3) % 6;
                    case RotationCommand.Left:
                        return (rightSide + 3) % 6;
                }
            }

            return -1;
        }

        private char GetCommandOfSide(int side, int downSide, int frontSide)
        {
            if (side == downSide)
                return RotationChars[(int)RotationCommand.Down];
            else if (side == frontSide)
                return RotationChars[(int)RotationCommand.Front];
            else if (side == (downSide + 3) % 6)
                return RotationChars[(int)RotationCommand.Up];
            else if (side == (frontSide + 3) % 6)
                return RotationChars[(int)RotationCommand.Back];
            else
            {
                int rightSide = Cube.RightSideForUpAndFront[(downSide + 3) % 6, frontSide];

                if (side == rightSide)
                    return RotationChars[(int)RotationCommand.Right];
                else
                    return RotationChars[(int)RotationCommand.Left];
            }
        }

        private void RotateSide(int side, bool reverse)
        {
            int[][,] rotations = Cube.GetRotations(side);
            for (int j = 0; j < 5; j++)
            {
                if (!reverse)
                {
                    int t = colors[rotations[j][0, 3], rotations[j][1, 3]];
                    for (int q = 3; q > 0; q--)
                    {
                        colors[rotations[j][0, q], rotations[j][1, q]] =
                            colors[rotations[j][0, q - 1], rotations[j][1, q - 1]];
                    }
                    colors[rotations[j][0, 0], rotations[j][1, 0]] = t;
                }
                else
                {
                    int t = colors[rotations[j][0, 0], rotations[j][1, 0]];
                    for (int q = 1; q < 4; q++)
                    {
                        colors[rotations[j][0, q - 1], rotations[j][1, q - 1]] =
                            colors[rotations[j][0, q], rotations[j][1, q]];
                    }
                    colors[rotations[j][0, 3], rotations[j][1, 3]] = t;
                }
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            string[] s = new string[6 * 9];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    s[i * 9 + j] = colors[i, j].ToString();
                }
            }
            Properties.Settings.Default.LastCube = String.Join(" ", s);
            Properties.Settings.Default.Save();
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            cube3DPictureBox.Invalidate();
        }

        private void frontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoRotationCommand((sender as ToolStripMenuItem).ShortcutKeyDisplayString[0]);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorsEntered = new int[6, 9];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    colorsEntered[i, j] = 0;
                }
            }
            DoColorsInput();
            cube3DPictureBox.Invalidate();
        }

        private void baseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorsEntered = new int[6, 9];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    colorsEntered[i, j] = 0;
                }
                colorsEntered[i, 8] = i + 1;
            }
            DoColorsInput();
            cube3DPictureBox.Invalidate();
        }

        private void fullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorsEntered = new int[6, 9];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    colorsEntered[i, j] = i + 1;
                }
            }
            DoColorsInput();
            cube3DPictureBox.Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetSelectedColor(int color)
        {
            for (int i = 0; i < colorToolStrip.Items.Count; i++)
            {
                (colorToolStrip.Items[i] as ToolStripButton).Checked = color == i + 1;
            }
            selectedColor = color;
        }

        private void colorToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int j = 0;
            for (int i = 0; i < colorToolStrip.Items.Count; i++)
            {
                if (colorToolStrip.Items[i] == e.ClickedItem)
                    j = i + 1;
            }
            SetSelectedColor(selectedColor == j ? 0 : j);
        }

        private void autocompleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoColorsInput();
            cube3DPictureBox.Invalidate();
        }

        private void mixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                int side = r.Next(6);
                bool reverse = r.NextDouble() >= 0.5;
                RotateSide(side, reverse);
            }
            colorsEntered = null;
            cube3DPictureBox.Invalidate();
        }

        private void solveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (colors[i, j] == 0)
                    {
                        MessageBox.Show("Specify all colors");
                        return;
                    }
                }
            }

            Color[] cubeColors = new Color[6];
            int[] colorReverseMap = new int[6];
            Dictionary<int, int> colorMap = new Dictionary<int, int>();
            for (int i = 0; i < 6; i++)
            {
                colorMap.Add(colors[i, 8], i);
                colorReverseMap[i] = colors[i, 8];
                cubeColors[i] = CurrentPalette[colors[i, 8] - 1];
            }

            Cube c = new Cube(false);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    c.Sides[i, j] = colorMap[colors[i, j]];
                }
            }

            CubeSolver solver;
            CubeSolutionGroup[] solutionGroups;
            try
            {
                CubeSolverFactory solverFactory = new CubeSolverFactory();
                solver = solverFactory.CreateCubeSolver();

                solutionGroups = solver.Solve(c);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to find solution: " + ex.Message);
                return;
            }

            if (solutionView == null)
            {
                solutionView = new SolutionViewForm();
                solutionView.Disposed += new EventHandler(delegate
                {
                    solutionView = null;
                });
                solutionView.Owner = this;
                solutionView.Show();

                if (this.Left > solutionView.Width)
                    solutionView.Left = this.Left - solutionView.Width + 10;
                else if (Screen.PrimaryScreen.WorkingArea.Width - this.Right > solutionView.Width)
                    solutionView.Left = this.Right - 10;
            }

            solutionView.Clear();

            for (int i = 0; i < solutionGroups.Length; i++)
            {
                SolutionViewCubeDraw drawIcon = new SolutionViewCubeDraw(solutionGroups[i].StartCube, CurrentPalette);

                Bitmap bmp = new Bitmap(48, 48);
                Graphics g = Graphics.FromImage(bmp);
                drawIcon.Draw(g, new RectangleF(-8, -8, bmp.Width + 16, bmp.Height + 16), 
                    GetTransformFor(solutionGroups[i].frontSide, solutionGroups[i].stage < 1));
                g.Dispose();

                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < solutionGroups[i].rotation.Length; j++)
                {
                    int q = solutionGroups[i].rotation[j];
                    char cmd = GetCommandOfSide(q < 0 ? ~q : q, solver.BaseSide, solutionGroups[i].frontSide);
                    sb.Append(Char.ToUpper(cmd));
                    if (q < 0) sb.Append('\'');
                    sb.Append(' ');
                }

                solutionView.AddLine("", sb.ToString(), bmp);
            }

            EnableControl(false);
            solutionWorker.RunWorkerAsync(solutionGroups);
        }

        private void EnableControl(bool enabled)
        {
            menuStrip1.Enabled = enabled;
            colorToolStrip.Enabled = enabled;
        }

        private void frontToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            rotation = Transform3D.NoRotation;
            cube3DPictureBox.Invalidate();
        }

        private void upPerspectiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rotation = UpPerspectiveRotation;
            cube3DPictureBox.Invalidate();
        }

        private void downPerspectiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rotation = DownPerspectiveRotation;
            cube3DPictureBox.Invalidate();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    CubeStorage loadedCube = CubeStorage.Load(openFileDialog1.FileName);
                    colorsEntered = new int[6, 9];
                    for (int i = 0; i < 6; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            colorsEntered[i, j] = loadedCube.SideColors[i][j];
                        }
                    }                    

                    if (loadedCube.Colors == null)
                    {
                        currentPalette = ChangePaletteForm.DefaultPalette;
                    }
                    else
                    {
                        Color[] palette = new Color[6];
                        for (int i = 0; i < palette.Length; i++)
                        {
                            palette[i] = Color.FromArgb(loadedCube.Colors[i]);
                        }
                        currentPalette = palette;
                    }

                    if (loadedCube.Rotation != null)
                    {
                        Matrix rotation = new Matrix(4, 4);
                        for (int i = 0; i < 16; i++)
                        {
                            rotation[i / 4, i % 4] = loadedCube.Rotation[i];
                        }
                        this.rotation = rotation;
                    }


                    DoColorsInput();
                    UpdateColorToolStrip();
                    cube3DPictureBox.Invalidate();
                }
                catch
                {
                    MessageBox.Show("Invalid data file");
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CubeStorage cubeToStore = new CubeStorage();
                cubeToStore.SideColors = new int[6][];
                for (int i = 0; i < 6; i++)
                {
                    cubeToStore.SideColors[i] = new int[9];
                    for (int j = 0; j < 9; j++)
                    {
                        cubeToStore.SideColors[i][j] = colors[i, j];
                    }
                }
                cubeToStore.Colors = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    cubeToStore.Colors[i] = CurrentPalette[i].ToArgb();
                }
                cubeToStore.Rotation = new double[16];
                for (int i = 0; i < 16; i++)
                {
                    cubeToStore.Rotation[i] = rotation[i / 4, i % 4];
                }
                cubeToStore.Save(saveFileDialog1.FileName);
            }
        }

        private double AproximateMovementPosition(int i, int total)
        {
            // simple: return (double)i / total;
            double value = (double)i * (cdfTable.Length - 1) / total;
            int low = (int)Math.Floor(value);
            int high = (int)Math.Ceiling(value);
            if (low == high) return cdfTable[low];

            return cdfTable[low] + (cdfTable[high] - cdfTable[low]) * (value - low) / (high - low);
        }

        private void moveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = (int)e.Argument;
            bool reverse = false;
            if (i < 0) { i = ~i; reverse = true; }

            side = i;
            int steps = 6;
            for (int j = 0; j < steps; j++)
            {
                sideAngle = Math.PI / 2 * AproximateMovementPosition(j, steps);
                if (reverse) sideAngle = -sideAngle;

                IAsyncResult r =
                    cube3DPictureBox.BeginInvoke(new CallbackDelegate(delegate { cube3DPictureBox.Refresh(); }));
                System.Threading.Thread.Sleep(50);
                cube3DPictureBox.EndInvoke(r);
            }

            side = null;
            RotateSide(i, reverse);
            cube3DPictureBox.Invalidate();
        }

        private void rotationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is int)
            {
                Matrix baseRotation = rotation;
                int steps = 5;
                for (int j = 0; j <= steps; j++)
                {
                    double alpha = Math.PI / 2 * AproximateMovementPosition(j, steps);
                    switch ((int)e.Argument)
                    {
                        case 0:
                            rotation = Transform3D.GetYRotation(-alpha) * baseRotation;
                            break;
                        case 1:
                            rotation = Transform3D.GetYRotation(alpha) * baseRotation;
                            break;
                        case 2:
                            rotation = Transform3D.GetXRotation(alpha) * baseRotation;
                            break;
                        case 3:
                            rotation = Transform3D.GetXRotation(-alpha) * baseRotation;
                            break;
                    }
                    IAsyncResult r =
                        cube3DPictureBox.BeginInvoke(new CallbackDelegate(delegate { cube3DPictureBox.Refresh(); }));
                    System.Threading.Thread.Sleep(50);
                    cube3DPictureBox.EndInvoke(r);
                }
            }
            else if(e.Argument is FrontSideRotationTask)
            {
                FrontSideRotationTask task = (FrontSideRotationTask)e.Argument;

                int steps = 9;
                for (int j = 0; j <= steps; j++)
                {
                    rotation = GetTransformForRotation(task.From, task.To,
                      AproximateMovementPosition(j, steps), task.DownView);
                    IAsyncResult r =
                        cube3DPictureBox.BeginInvoke(new CallbackDelegate(delegate { cube3DPictureBox.Refresh(); }));
                    System.Threading.Thread.Sleep(50);
                    cube3DPictureBox.EndInvoke(r);
                }
            }
        }

        private void rotateXXXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoRotate(int.Parse(((ToolStripMenuItem)sender).Tag as string));
        }

        private void DoRotate(int command)
        {
            if (!rotationWorker.IsBusy)
                rotationWorker.RunWorkerAsync(command);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                    DoRotate(0);
                    break;
                case Keys.Left:
                    DoRotate(1);
                    break;
                case Keys.Up:
                    DoRotate(2);
                    break;
                case Keys.Down:
                    DoRotate(3);
                    break;
                default:
                    return base.ProcessDialogKey(keyData);
            }
            return true;
        }

        private void solutionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Color[] cubeColors = new Color[6];
            int[] colorReverseMap = new int[6];
            for (int i = 0; i < 6; i++)
            {
                colorReverseMap[i] = colors[i, 8];
            }

            CubeSolutionGroup[] sol = (CubeSolutionGroup[])e.Argument;
            //Cube c = sol[sol.Length - 1].StartCube;
            //for (int i = 0; i < 6; i++)
            //{
            //    for (int j = 0; j < 8; j++)
            //    {
            //        colors[i, j] = colorReverseMap[c.Sides[i, j]];
            //    }
            //}
            //colorsEntered = null;
            //pictureBox1.Invalidate();

            rotation = DownPerspectiveRotation;

            cube3DPictureBox.Invalidate();
            int currentFront = 0;
            bool downView = true;

            for (int i = 0; i < sol.Length; i++)
            {
                CubeSolutionGroup grp = sol[i];

                if (downView && grp.stage > 0)
                {
                    rotationWorker.RunWorkerAsync(3);
                    while (rotationWorker.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    downView = false;
                }

                if (currentFront != grp.frontSide)
                {
                    rotationWorker.RunWorkerAsync(new FrontSideRotationTask(currentFront, grp.frontSide, downView));
                    while (rotationWorker.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    currentFront = grp.frontSide;
                }

                for (int j = 0; j < grp.rotation.Length; j++)
                {
                    moveWorker.RunWorkerAsync(grp.rotation[j]);
                    while (moveWorker.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }

            }
        }

        private void captureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (CaptureForm form = new CaptureForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int[] t = CubeSidesColorOffset.FindTransform(form.Colors);

                    Color[] palette = new Color[6];
                    for (int i = 0; i < palette.Length; i++)
                    {
                        palette[t[i]] = form.Colors[i];
                    }
                    currentPalette = palette;

                    int[,] data = new int[6, 9];
                    for (int i = 0; i < 6; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            data[t[i], j] = t[form.Cube[i, j]] + 1;
                        }
                    }
                    colorsEntered = data;

                    DoColorsInput();
                    UpdateColorToolStrip();
                }
            }
        }

        private void changePaletteMenuItem_Click(object sender, EventArgs e)
        {
            using (ChangePaletteForm form = new ChangePaletteForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                }
            }
        }

        delegate void CallbackDelegate();

        private class ScreenCubeDraw : Cube2DDraw
        {
            private MainForm form;

            public ScreenCubeDraw(MainForm form) { this.form = form; }

            public override Color GetSidePieceColor(int side, int piece)
            {
                if (piece < 0 || form.colors[side, piece] == 0)
                    return EmptySideColor;
                else
                    return form.CurrentPalette[form.colors[side, piece] - 1];
            }
        }

        private class SolutionViewCubeDraw : Cube2DDraw
        {
            private Cube cube;
            private Color[] palette;

            public SolutionViewCubeDraw(Cube cube, Color[] palette)
            {
                this.cube = cube;
                this.palette = palette;
            }

            public override Color GetSidePieceColor(int side, int piece)
            {
                if (piece < 0)
                    return EmptySideColor;
                else if (piece < 8)
                    return palette[cube[side, piece]];
                else
                    return palette[side];
            }
        }

        private class PointFXComparer : IComparer<PointF>
        {
            public int Compare(PointF x, PointF y)
            {
                return Comparer<float>.Default.Compare(x.X, y.X);
            }
        }

        private class FrontSideRotationTask
        {
            public int From;
            public int To;
            public bool DownView;

            public FrontSideRotationTask(int from, int to, bool downView)
            {
                this.From = from;
                this.To = to;
                this.DownView = downView;
            }
        }

        private void solutionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableControl(true);
        }
    }
}