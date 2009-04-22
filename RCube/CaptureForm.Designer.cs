namespace RCube
{
    partial class CaptureForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.capturedSidesListView = new System.Windows.Forms.ListView();
            this.sidesImageList = new System.Windows.Forms.ImageList(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.clickOnceButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.colorsListBox = new System.Windows.Forms.ListBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.sideContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.workToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.captureStateToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.solutionDetectionBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.changeColorDialog = new System.Windows.Forms.ColorDialog();
            this.cubeCapture = new RCubeCapture.CubeCapture();
            this.statusStrip.SuspendLayout();
            this.sideContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // capturedSidesListView
            // 
            this.capturedSidesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.capturedSidesListView.ContextMenuStrip = this.sideContextMenuStrip;
            this.capturedSidesListView.LargeImageList = this.sidesImageList;
            this.capturedSidesListView.Location = new System.Drawing.Point(12, 12);
            this.capturedSidesListView.Name = "capturedSidesListView";
            this.capturedSidesListView.Size = new System.Drawing.Size(449, 286);
            this.capturedSidesListView.TabIndex = 0;
            this.capturedSidesListView.UseCompatibleStateImageBehavior = false;
            this.capturedSidesListView.SelectedIndexChanged += new System.EventHandler(this.capturedSidesListView_SelectedIndexChanged);
            // 
            // sidesImageList
            // 
            this.sidesImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.sidesImageList.ImageSize = new System.Drawing.Size(96, 96);
            this.sidesImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(467, 275);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // finishButton
            // 
            this.finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.finishButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.finishButton.Enabled = false;
            this.finishButton.Location = new System.Drawing.Point(467, 246);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 2;
            this.finishButton.Text = "&Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            // 
            // clickOnceButton
            // 
            this.clickOnceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clickOnceButton.Location = new System.Drawing.Point(468, 13);
            this.clickOnceButton.Name = "clickOnceButton";
            this.clickOnceButton.Size = new System.Drawing.Size(75, 23);
            this.clickOnceButton.TabIndex = 3;
            this.clickOnceButton.Text = "Click &Once";
            this.clickOnceButton.UseVisualStyleBackColor = true;
            this.clickOnceButton.Click += new System.EventHandler(this.clickOnceButton_Click);
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startButton.Location = new System.Drawing.Point(468, 43);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 4;
            this.startButton.Text = "&Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(468, 73);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 5;
            this.stopButton.Text = "S&top";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(467, 102);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 6;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // colorsListBox
            // 
            this.colorsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.colorsListBox.ColumnWidth = 35;
            this.colorsListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.colorsListBox.FormattingEnabled = true;
            this.colorsListBox.ItemHeight = 30;
            this.colorsListBox.Location = new System.Drawing.Point(468, 146);
            this.colorsListBox.MultiColumn = true;
            this.colorsListBox.Name = "colorsListBox";
            this.colorsListBox.Size = new System.Drawing.Size(74, 94);
            this.colorsListBox.TabIndex = 7;
            this.colorsListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.colorsListBox_DrawItem);
            this.colorsListBox.DoubleClick += new System.EventHandler(this.colorsListBox_DoubleClick);
            // 
            // statusStrip
            // 
            this.statusStrip.ContextMenuStrip = this.sideContextMenuStrip;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusToolStripStatusLabel,
            this.workToolStripProgressBar,
            this.captureStateToolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 318);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(554, 22);
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "statusStrip1";
            // 
            // sideContextMenuStrip
            // 
            this.sideContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.toolStripMenuItem1,
            this.deleteToolStripMenuItem});
            this.sideContextMenuStrip.Name = "sideContextMenuStrip";
            this.sideContextMenuStrip.Size = new System.Drawing.Size(139, 54);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.editToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(135, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // statusToolStripStatusLabel
            // 
            this.statusToolStripStatusLabel.AutoSize = false;
            this.statusToolStripStatusLabel.Name = "statusToolStripStatusLabel";
            this.statusToolStripStatusLabel.Size = new System.Drawing.Size(150, 17);
            // 
            // workToolStripProgressBar
            // 
            this.workToolStripProgressBar.Name = "workToolStripProgressBar";
            this.workToolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // captureStateToolStripStatusLabel
            // 
            this.captureStateToolStripStatusLabel.Name = "captureStateToolStripStatusLabel";
            this.captureStateToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // solutionDetectionBackgroundWorker
            // 
            this.solutionDetectionBackgroundWorker.WorkerReportsProgress = true;
            this.solutionDetectionBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.solutionDetectionBackgroundWorker_DoWork);
            this.solutionDetectionBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.solutionDetectionBackgroundWorker_RunWorkerCompleted);
            this.solutionDetectionBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.solutionDetectionBackgroundWorker_ProgressChanged);
            // 
            // cubeCapture
            // 
            this.cubeCapture.CaptureSound = "media\\click.wav";
            this.cubeCapture.CaptureClick += new RCubeCapture.CaptureClickEventHandler(this.cubeCapture_CaptureClick);
            this.cubeCapture.Closed += new System.EventHandler(this.cubeCapture_Closed);
            this.cubeCapture.CameraError += new System.EventHandler(this.cubeCapture_CameraError);
            // 
            // CaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 340);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.colorsListBox);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.clickOnceButton);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.capturedSidesListView);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 350);
            this.Name = "CaptureForm";
            this.Text = "Capture Cube";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CaptureForm_FormClosed);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.sideContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView capturedSidesListView;
        private System.Windows.Forms.ImageList sidesImageList;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.Button clickOnceButton;
        private RCubeCapture.CubeCapture cubeCapture;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.ListBox colorsListBox;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusToolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar workToolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel captureStateToolStripStatusLabel;
        private System.ComponentModel.BackgroundWorker solutionDetectionBackgroundWorker;
        private System.Windows.Forms.ColorDialog changeColorDialog;
        private System.Windows.Forms.ContextMenuStrip sideContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}