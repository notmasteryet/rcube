using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;

namespace RCube
{
    /// <summary>
    /// Captures cube from camera. Allows editing of captures.
    /// </summary>
    public partial class CaptureForm : Form
    {
        CaptureState state = CaptureState.Standby;
        private volatile bool continueWork = false;
        private Color[] currentColors = null;
        private int[,] sidesColorIndexes = null;
        private int[,] currentSolution = null;
        private SoundPlayer tadaPlayer = null;

        /// <summary>
        /// Gets captured colors.
        /// </summary>
        public Color[] Colors
        {
            get { return currentColors; }
        }

        /// <summary>
        /// Gets captured cube configuration.
        /// </summary>
        public int[,] Cube
        {
            get { return currentSolution; }
        }

        public CaptureForm()
        {
            InitializeComponent();
        }

        private List<int[,]> sides = new List<int[,]>();

        private void clickOnceButton_Click(object sender, EventArgs e)
        {
            state = CaptureState.ClickOnce;
            UpdateCaptureButtons();

            cubeCapture.Show();
        }

        private void UpdateCaptureButtons()
        {
            clickOnceButton.Enabled = state == CaptureState.Standby;
            startButton.Enabled = state == CaptureState.Standby;
            stopButton.Enabled = state == CaptureState.ClickOnce || state == CaptureState.Capture;

            switch (state)
            {
                case CaptureState.ClickOnce:
                    captureStateToolStripStatusLabel.Text = "ClickOnce";
                    break;
                case CaptureState.Capture:
                    captureStateToolStripStatusLabel.Text = "Capturing";
                    break;
                default:
                    captureStateToolStripStatusLabel.Text = String.Empty;
                    break;
            }
        }

        private void AddSide(int[,] side)
        {
            int index = sides.Count;

            sides.Add(side);

            Image bmp = DrawSideImage(side);
            sidesImageList.Images.Add(bmp);

            ListViewItem li = new ListViewItem();
            li.ImageIndex = index;

            capturedSidesListView.Items.Add(li);

            if (sides.Count >= 6)
            {
                if (solutionDetectionBackgroundWorker.IsBusy)
                    continueWork = true;
                else
                    solutionDetectionBackgroundWorker.RunWorkerAsync();
            }
        }

        private Image DrawSideImage(int[,] side)
        {
            Bitmap bmp = new Bitmap(96, 96);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int q = i * 3 + j;
                        Color c = Color.FromArgb(side[q, 0], side[q, 1], side[q, 2]);
                        g.FillRectangle(new SolidBrush(c),
                            j * 32 + 1, i * 32 + 1, 30, 30);
                    }
                }
            }
            return bmp;
        }

        private Image DrawSideImage(int index)
        {
            Bitmap bmp = new Bitmap(96, 96);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int q = i * 3 + j;
                        int colorIndex = sidesColorIndexes[index, q];
                        g.FillRectangle(new SolidBrush(currentColors[colorIndex]),
                            j * 32 + 1, i * 32 + 1, 30, 30);
                    }
                }
            }
            return bmp;
        }

        private void RemoveSideAt(int index)
        {
            sides.RemoveAt(index);

            Image sideImage = sidesImageList.Images[index];
            sidesImageList.Images.RemoveAt(index);
            sideImage.Dispose();

            capturedSidesListView.Items.RemoveAt(index);
        }

        private void cubeCapture_CaptureClick(object sender, RCubeCapture.CaptureClickEventArgs e)
        {
            this.BeginInvoke(new InvokeHandler(delegate
            {
                AddSide(e.Colors);

                if (state == CaptureState.ClickOnce)
                {
                    stopButton_Click(this, EventArgs.Empty);
                }
            }));
        }

        private void colorsListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (currentColors == null || e.Index < 0) return;

            e.Graphics.FillRectangle(new SolidBrush(currentColors[e.Index]),
                e.Bounds);
            e.DrawFocusRectangle();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            cubeCapture.Hide();

            state = CaptureState.Standby;
            UpdateCaptureButtons();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            state = CaptureState.Capture;
            UpdateCaptureButtons();

            cubeCapture.Show();
        }

        private void CaptureForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (cubeCapture.IsBusy)
            {
                cubeCapture.Hide();
            }
        }

        private void solutionDetectionBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            const int colorDetectionPercentange = 20;
            continueWork = false;

            SolutionDetectionResults results = new SolutionDetectionResults();
            int[,] sidesColorIndexes = (int[,])e.Argument;

            if (sidesColorIndexes == null)
            {
                // run color detection
                solutionDetectionBackgroundWorker.ReportProgress(0);

                int[,] colors;
                RCubeCapture.ColorGroups.GetColorGroups(sides, out colors, out sidesColorIndexes);

                results.colors = new Color[6];
                for (int i = 0; i < 6; i++)
                {
                    results.colors[i] = Color.FromArgb(colors[i, 0], colors[i, 1], colors[i, 2]);
                }
            }
                
            solutionDetectionBackgroundWorker.ReportProgress(colorDetectionPercentange);

            bool moreThanOne;
            int[,] solution = CubeMaker.Make(sidesColorIndexes, delegate(int percentange)
            {
                solutionDetectionBackgroundWorker.ReportProgress(colorDetectionPercentange
                    + percentange * (100 - colorDetectionPercentange) / 100);
            }, out moreThanOne);

            results.sidesColorIndexes = sidesColorIndexes;
            if (solution != null && !moreThanOne)
            {
                results.solution = solution;
            }
            e.Result = results;

            solutionDetectionBackgroundWorker.ReportProgress(100);
        }

        private void solutionDetectionBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            workToolStripProgressBar.Value = e.ProgressPercentage;
        }

        private void solutionDetectionBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            workToolStripProgressBar.Value = 0;

            if(e.Cancelled || e.Error != null) return;

            SolutionDetectionResults results = (SolutionDetectionResults)e.Result;

            if (results.colors != null)
            {
                currentColors = results.colors;
                colorsListBox.Items.Clear();
                for (int i = 0; i < currentColors.Length; i++)
                {
                    colorsListBox.Items.Add(currentColors[i]);
                }
                colorsListBox.Invalidate();
            }

            sidesColorIndexes = results.sidesColorIndexes;
            UpdateSideColors();

            SetSolution(results.solution);

            if (continueWork)
            {
                solutionDetectionBackgroundWorker.RunWorkerAsync();
            }
        }

        private void SetSolution(int[,] solution)
        {
            bool wasSolution = currentSolution != null;
            bool solutionFound = solution != null;
            currentSolution = solution;
            finishButton.Enabled = solutionFound;

            statusToolStripStatusLabel.Text = solutionFound ?
                "Layout was found" : "";

            if (!wasSolution && solutionFound)
            {
                GetTadaPlayer().Play();
            }
        }

        private SoundPlayer GetTadaPlayer()
        {
            if (tadaPlayer == null)
            {
                tadaPlayer = new SoundPlayer(@"media\tada.wav");
            }
            return tadaPlayer;
        }

        private void UpdateSideColors()
        {
            if (sidesColorIndexes != null)
            {
                for (int i = 0; i < sidesColorIndexes.GetLength(0); i++)
                {
                    Image oldImage = sidesImageList.Images[i];
                    Image newImage = DrawSideImage(i);
                    sidesImageList.Images[i] = newImage;
                    oldImage.Dispose();
                }
            }
            capturedSidesListView.Invalidate();
        }

#region "Addition classes and delegates"
        private delegate void InvokeHandler();

        private enum CaptureState
        {
            Standby,
            ClickOnce,
            Capture,
            Disabled
        }

        private class SolutionDetectionResults
        {
            public Color[] colors;
            public int[,] sidesColorIndexes;
            public int[,] solution;
        }
#endregion

        private void colorsListBox_DoubleClick(object sender, EventArgs e)
        {
            int colorIndex = colorsListBox.SelectedIndex;
            if(colorIndex < 0) return;

            changeColorDialog.Color = currentColors[colorIndex];
            if (changeColorDialog.ShowDialog() == DialogResult.OK)
            {
                currentColors[colorIndex] = changeColorDialog.Color;
                colorsListBox.Invalidate();

                UpdateSideColors();
            }
        }

        private void cubeCapture_CameraError(object sender, EventArgs e)
        {
            cubeCapture.Hide();

            BeginInvoke(new InvokeHandler(delegate
            {
                state = CaptureState.Standby;
                UpdateCaptureButtons();
            }));
        }

        private void cubeCapture_Closed(object sender, EventArgs e)
        {
            BeginInvoke(new InvokeHandler(delegate
            {
                state = CaptureState.Standby;
                UpdateCaptureButtons();
            }));
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            sides.Clear();
            sidesImageList.Images.Clear();
            capturedSidesListView.Items.Clear();

            SetSolution(null);
            sidesColorIndexes = null;
            currentColors = null;
            UpdateSideColors();
        }

        private void capturedSidesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            editToolStripMenuItem.Enabled = capturedSidesListView.SelectedIndices.Count == 1;
            deleteToolStripMenuItem.Enabled = capturedSidesListView.SelectedIndices.Count > 0;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capturedSidesListView.SelectedIndices.Count > 0)
            {
                int index = capturedSidesListView.SelectedIndices[0];

                Rectangle r = capturedSidesListView.GetItemRect(index);
                
                CaptureSideEditForm f = new CaptureSideEditForm();
                
                f.Location = capturedSidesListView.PointToScreen(new Point(r.Left - 32, r.Top - 32));
                f.Colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Beige, Color.White, Color.Orange };
                f.SideColors = new int[] { 1, 2, 3, 4, 5, 0, 1, 2, 5 };
                f.Show();
                // TODO edit
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capturedSidesListView.SelectedIndices.Count > 0)
            {                
                int[] indicesToRemove = new int[capturedSidesListView.SelectedIndices.Count];
                capturedSidesListView.SelectedIndices.CopyTo(indicesToRemove, 0);
                Array.Sort(indicesToRemove);

                for (int i = indicesToRemove.Length - 1; i >= 0; i--)
                {
                    RemoveSideAt(indicesToRemove[i]);
                }
            }

        }
    }
}