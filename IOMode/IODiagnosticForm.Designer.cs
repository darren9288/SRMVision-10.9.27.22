namespace IOMode
{
    partial class IODiagnosticForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IODiagnosticForm));
            this.IO2RadioButton = new SRMControl.SRMRadioButton();
            this.PositiveCheckBox = new SRMControl.SRMCheckBox();
            this.IO1RadioButton = new SRMControl.SRMRadioButton();
            this.TriggerSignalGroupBox = new SRMControl.SRMGroupBox();
            this.IO4RadioButton = new SRMControl.SRMRadioButton();
            this.IO3RadioButton = new SRMControl.SRMRadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.ScaleTrackBar = new System.Windows.Forms.TrackBar();
            this.StartButton = new SRMControl.SRMButton();
            this.StopButton = new SRMControl.SRMButton();
            this.SignalGraph = new SRMControl.SRMSignalGraph();
            this.CloseButton = new SRMControl.SRMButton();
            this.ShowGridChB = new SRMControl.SRMCheckBox();
            this.ContinueScanCheckBox = new SRMControl.SRMCheckBox();
            this.TriggerSignalGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScaleTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // IO2RadioButton
            // 
            resources.ApplyResources(this.IO2RadioButton, "IO2RadioButton");
            this.IO2RadioButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.IO2RadioButton.Name = "IO2RadioButton";
            this.IO2RadioButton.UseVisualStyleBackColor = false;
            // 
            // PositiveCheckBox
            // 
            resources.ApplyResources(this.PositiveCheckBox, "PositiveCheckBox");
            this.PositiveCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.PositiveCheckBox.Checked = true;
            this.PositiveCheckBox.CheckedColor = System.Drawing.Color.GreenYellow;
            this.PositiveCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PositiveCheckBox.Name = "PositiveCheckBox";
            this.PositiveCheckBox.Selected = false;
            this.PositiveCheckBox.SelectedBorderColor = System.Drawing.Color.Red;
            this.PositiveCheckBox.UnCheckedColor = System.Drawing.Color.Red;
            this.PositiveCheckBox.UseVisualStyleBackColor = false;
            // 
            // IO1RadioButton
            // 
            resources.ApplyResources(this.IO1RadioButton, "IO1RadioButton");
            this.IO1RadioButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.IO1RadioButton.Name = "IO1RadioButton";
            this.IO1RadioButton.UseVisualStyleBackColor = false;
            // 
            // TriggerSignalGroupBox
            // 
            resources.ApplyResources(this.TriggerSignalGroupBox, "TriggerSignalGroupBox");
            this.TriggerSignalGroupBox.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.TriggerSignalGroupBox.Controls.Add(this.IO4RadioButton);
            this.TriggerSignalGroupBox.Controls.Add(this.IO3RadioButton);
            this.TriggerSignalGroupBox.Controls.Add(this.IO1RadioButton);
            this.TriggerSignalGroupBox.Controls.Add(this.IO2RadioButton);
            this.TriggerSignalGroupBox.Name = "TriggerSignalGroupBox";
            this.TriggerSignalGroupBox.TabStop = false;
            // 
            // IO4RadioButton
            // 
            resources.ApplyResources(this.IO4RadioButton, "IO4RadioButton");
            this.IO4RadioButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.IO4RadioButton.Name = "IO4RadioButton";
            this.IO4RadioButton.UseVisualStyleBackColor = false;
            // 
            // IO3RadioButton
            // 
            resources.ApplyResources(this.IO3RadioButton, "IO3RadioButton");
            this.IO3RadioButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.IO3RadioButton.Name = "IO3RadioButton";
            this.IO3RadioButton.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ScaleTrackBar
            // 
            resources.ApplyResources(this.ScaleTrackBar, "ScaleTrackBar");
            this.ScaleTrackBar.LargeChange = 1;
            this.ScaleTrackBar.Maximum = 7;
            this.ScaleTrackBar.Minimum = 1;
            this.ScaleTrackBar.Name = "ScaleTrackBar";
            this.ScaleTrackBar.Value = 5;
            this.ScaleTrackBar.Scroll += new System.EventHandler(this.ScaleTrackBar_Scroll);
            // 
            // StartButton
            // 
            resources.ApplyResources(this.StartButton, "StartButton");
            this.StartButton.ForeColor = System.Drawing.Color.MidnightBlue;
            this.StartButton.Name = "StartButton";
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // StopButton
            // 
            resources.ApplyResources(this.StopButton, "StopButton");
            this.StopButton.ForeColor = System.Drawing.Color.MidnightBlue;
            this.StopButton.Name = "StopButton";
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // SignalGraph
            // 
            resources.ApplyResources(this.SignalGraph, "SignalGraph");
            this.SignalGraph.BackColor = System.Drawing.Color.Black;
            this.SignalGraph.ForeColor = System.Drawing.Color.Gold;
            this.SignalGraph.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.SignalGraph.GridDashSpace = 1;
            this.SignalGraph.GridSize = SRMControl.SRMGraphGridSize.Small;
            this.SignalGraph.HeaderText = "Header";
            this.SignalGraph.Name = "SignalGraph";
            this.SignalGraph.ShowGraphGrid = true;
            this.SignalGraph.Signal1Color = System.Drawing.Color.Red;
            this.SignalGraph.Signal1Text = "Signal 1";
            this.SignalGraph.Signal1TextColor = System.Drawing.Color.Red;
            this.SignalGraph.Signal1Visible = true;
            this.SignalGraph.Signal2Color = System.Drawing.Color.LawnGreen;
            this.SignalGraph.Signal2Text = "Signal 2";
            this.SignalGraph.Signal2TextColor = System.Drawing.Color.LawnGreen;
            this.SignalGraph.Signal2Visible = true;
            this.SignalGraph.Signal3Color = System.Drawing.Color.Cyan;
            this.SignalGraph.Signal3Text = "Signal 3";
            this.SignalGraph.Signal3TextColor = System.Drawing.Color.Cyan;
            this.SignalGraph.Signal3Visible = true;
            this.SignalGraph.Signal4Color = System.Drawing.Color.Yellow;
            this.SignalGraph.Signal4Text = "Signal 4";
            this.SignalGraph.Signal4TextColor = System.Drawing.Color.Yellow;
            this.SignalGraph.Signal4Visible = true;
            this.SignalGraph.SignalTotal = 4;
            this.SignalGraph.UnitPerDivision = 10F;
            this.SignalGraph.XAxisText = "Time (10 ms/d)";
            this.SignalGraph.XAxisTextColor = System.Drawing.Color.MediumOrchid;
            this.SignalGraph.XYAxisFont = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SignalGraph.YAxisText = "Signal";
            this.SignalGraph.YAxisTextColor = System.Drawing.Color.MediumOrchid;
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.ForeColor = System.Drawing.Color.MidnightBlue;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // ShowGridChB
            // 
            resources.ApplyResources(this.ShowGridChB, "ShowGridChB");
            this.ShowGridChB.Checked = true;
            this.ShowGridChB.CheckedColor = System.Drawing.Color.Empty;
            this.ShowGridChB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowGridChB.Name = "ShowGridChB";
            this.ShowGridChB.Selected = false;
            this.ShowGridChB.SelectedBorderColor = System.Drawing.Color.Red;
            this.ShowGridChB.UnCheckedColor = System.Drawing.Color.Empty;
            this.ShowGridChB.CheckedChanged += new System.EventHandler(this.ShowGridChB_CheckedChanged);
            // 
            // ContinueScanCheckBox
            // 
            resources.ApplyResources(this.ContinueScanCheckBox, "ContinueScanCheckBox");
            this.ContinueScanCheckBox.Checked = true;
            this.ContinueScanCheckBox.CheckedColor = System.Drawing.Color.Empty;
            this.ContinueScanCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ContinueScanCheckBox.Name = "ContinueScanCheckBox";
            this.ContinueScanCheckBox.Selected = false;
            this.ContinueScanCheckBox.SelectedBorderColor = System.Drawing.Color.Red;
            this.ContinueScanCheckBox.UnCheckedColor = System.Drawing.Color.Empty;
            this.ContinueScanCheckBox.CheckedChanged += new System.EventHandler(this.ContinueScanCheckBox_CheckedChanged);
            // 
            // IODiagnosticForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.ContinueScanCheckBox);
            this.Controls.Add(this.PositiveCheckBox);
            this.Controls.Add(this.TriggerSignalGroupBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.ShowGridChB);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.SignalGraph);
            this.Controls.Add(this.ScaleTrackBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IODiagnosticForm";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IODiagnosticForm_FormClosing);
            this.TriggerSignalGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ScaleTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMCheckBox PositiveCheckBox;
        private SRMControl.SRMGroupBox TriggerSignalGroupBox;
        private SRMControl.SRMRadioButton IO1RadioButton;
        private SRMControl.SRMRadioButton IO2RadioButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar ScaleTrackBar;
        private SRMControl.SRMButton StartButton;
        private SRMControl.SRMButton StopButton;
        private SRMControl.SRMSignalGraph SignalGraph;
        private SRMControl.SRMButton CloseButton;
        private SRMControl.SRMRadioButton IO4RadioButton;
        private SRMControl.SRMRadioButton IO3RadioButton;
        private SRMControl.SRMCheckBox ContinueScanCheckBox;
        private SRMControl.SRMCheckBox ShowGridChB;
    }
}