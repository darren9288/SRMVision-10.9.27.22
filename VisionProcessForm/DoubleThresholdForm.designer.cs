namespace VisionProcessForm
{
    partial class DoubleThresholdForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DoubleThresholdForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.group_Threshold = new SRMControl.SRMGroupBox();
            this.pnl_Threshold = new System.Windows.Forms.Panel();
            this.pnl_MiddleThreshold = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.pnl_HighThreshold = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pnl_LowThreshold = new System.Windows.Forms.Panel();
            this.txt_LowThreshold = new SRMControl.SRMInputBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_HighThreshold = new SRMControl.SRMInputBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.pic_Histogram = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel_Threshold = new System.Windows.Forms.Panel();
            this.panel_Gain = new System.Windows.Forms.Panel();
            this.chk_ViewGrayImage = new SRMControl.SRMCheckBox();
            this.txt_ImageGain = new System.Windows.Forms.NumericUpDown();
            this.srmLabel26 = new SRMControl.SRMLabel();
            this.panel_Histogram = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chk_SetToOtherSideROIs = new SRMControl.SRMCheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pic_ROI = new System.Windows.Forms.PictureBox();
            this.group_Threshold.SuspendLayout();
            this.pnl_Threshold.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Histogram)).BeginInit();
            this.panel_Threshold.SuspendLayout();
            this.panel_Gain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_ImageGain)).BeginInit();
            this.panel_Histogram.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ROI)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // group_Threshold
            // 
            resources.ApplyResources(this.group_Threshold, "group_Threshold");
            this.group_Threshold.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_Threshold.Controls.Add(this.pnl_Threshold);
            this.group_Threshold.Controls.Add(this.txt_LowThreshold);
            this.group_Threshold.Controls.Add(this.srmLabel1);
            this.group_Threshold.Controls.Add(this.txt_HighThreshold);
            this.group_Threshold.Controls.Add(this.srmLabel2);
            this.group_Threshold.Name = "group_Threshold";
            this.group_Threshold.TabStop = false;
            // 
            // pnl_Threshold
            // 
            resources.ApplyResources(this.pnl_Threshold, "pnl_Threshold");
            this.pnl_Threshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_Threshold.Controls.Add(this.pnl_MiddleThreshold);
            this.pnl_Threshold.Controls.Add(this.splitter2);
            this.pnl_Threshold.Controls.Add(this.pnl_HighThreshold);
            this.pnl_Threshold.Controls.Add(this.splitter1);
            this.pnl_Threshold.Controls.Add(this.pnl_LowThreshold);
            this.pnl_Threshold.Name = "pnl_Threshold";
            // 
            // pnl_MiddleThreshold
            // 
            resources.ApplyResources(this.pnl_MiddleThreshold, "pnl_MiddleThreshold");
            this.pnl_MiddleThreshold.BackColor = System.Drawing.Color.White;
            this.pnl_MiddleThreshold.Name = "pnl_MiddleThreshold";
            // 
            // splitter2
            // 
            resources.ApplyResources(this.splitter2, "splitter2");
            this.splitter2.BackColor = System.Drawing.Color.Silver;
            this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter2.Name = "splitter2";
            this.splitter2.TabStop = false;
            this.splitter2.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter_SplitterMoved);
            this.splitter2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitter2_MouseDown);
            this.splitter2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.splitter2_MouseMove);
            // 
            // pnl_HighThreshold
            // 
            resources.ApplyResources(this.pnl_HighThreshold, "pnl_HighThreshold");
            this.pnl_HighThreshold.BackColor = System.Drawing.Color.Black;
            this.pnl_HighThreshold.Name = "pnl_HighThreshold";
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.BackColor = System.Drawing.Color.Silver;
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            this.splitter1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter_SplitterMoved);
            this.splitter1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitter1_MouseDown);
            this.splitter1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.splitter1_MouseMove);
            // 
            // pnl_LowThreshold
            // 
            resources.ApplyResources(this.pnl_LowThreshold, "pnl_LowThreshold");
            this.pnl_LowThreshold.BackColor = System.Drawing.Color.Black;
            this.pnl_LowThreshold.Name = "pnl_LowThreshold";
            // 
            // txt_LowThreshold
            // 
            resources.ApplyResources(this.txt_LowThreshold, "txt_LowThreshold");
            this.txt_LowThreshold.BackColor = System.Drawing.Color.White;
            this.txt_LowThreshold.DecimalPlaces = 0;
            this.txt_LowThreshold.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_LowThreshold.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_LowThreshold.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_LowThreshold.ForeColor = System.Drawing.Color.Black;
            this.txt_LowThreshold.InputType = SRMControl.InputType.Number;
            this.txt_LowThreshold.Name = "txt_LowThreshold";
            this.txt_LowThreshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_LowThreshold.TextChanged += new System.EventHandler(this.txt_LowThreshold_TextChanged);
            this.txt_LowThreshold.Leave += new System.EventHandler(this.txt_LowThreshold_Leave);
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_HighThreshold
            // 
            resources.ApplyResources(this.txt_HighThreshold, "txt_HighThreshold");
            this.txt_HighThreshold.BackColor = System.Drawing.Color.White;
            this.txt_HighThreshold.DecimalPlaces = 0;
            this.txt_HighThreshold.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_HighThreshold.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_HighThreshold.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_HighThreshold.ForeColor = System.Drawing.Color.Black;
            this.txt_HighThreshold.InputType = SRMControl.InputType.Number;
            this.txt_HighThreshold.Name = "txt_HighThreshold";
            this.txt_HighThreshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_HighThreshold.TextChanged += new System.EventHandler(this.txt_HighThreshold_TextChanged);
            this.txt_HighThreshold.Leave += new System.EventHandler(this.txt_HighThreshold_Leave);
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pic_Histogram
            // 
            resources.ApplyResources(this.pic_Histogram, "pic_Histogram");
            this.pic_Histogram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.pic_Histogram.Name = "pic_Histogram";
            this.pic_Histogram.TabStop = false;
            this.pic_Histogram.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_Histogram_Paint);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel_Threshold
            // 
            resources.ApplyResources(this.panel_Threshold, "panel_Threshold");
            this.panel_Threshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.panel_Threshold.Controls.Add(this.group_Threshold);
            this.panel_Threshold.Name = "panel_Threshold";
            // 
            // panel_Gain
            // 
            resources.ApplyResources(this.panel_Gain, "panel_Gain");
            this.panel_Gain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.panel_Gain.Controls.Add(this.chk_ViewGrayImage);
            this.panel_Gain.Controls.Add(this.txt_ImageGain);
            this.panel_Gain.Controls.Add(this.srmLabel26);
            this.panel_Gain.Name = "panel_Gain";
            // 
            // chk_ViewGrayImage
            // 
            resources.ApplyResources(this.chk_ViewGrayImage, "chk_ViewGrayImage");
            this.chk_ViewGrayImage.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_ViewGrayImage.Name = "chk_ViewGrayImage";
            this.chk_ViewGrayImage.Selected = false;
            this.chk_ViewGrayImage.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_ViewGrayImage.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_ViewGrayImage.UseVisualStyleBackColor = true;
            this.chk_ViewGrayImage.Click += new System.EventHandler(this.chk_ViewGrayImage_Click);
            // 
            // txt_ImageGain
            // 
            resources.ApplyResources(this.txt_ImageGain, "txt_ImageGain");
            this.txt_ImageGain.DecimalPlaces = 1;
            this.txt_ImageGain.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.txt_ImageGain.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_ImageGain.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_ImageGain.Name = "txt_ImageGain";
            this.txt_ImageGain.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_ImageGain.ValueChanged += new System.EventHandler(this.txt_ImageGain_ValueChanged);
            // 
            // srmLabel26
            // 
            resources.ApplyResources(this.srmLabel26, "srmLabel26");
            this.srmLabel26.Name = "srmLabel26";
            this.srmLabel26.TextShadowColor = System.Drawing.Color.Gray;
            this.srmLabel26.UseWaitCursor = true;
            // 
            // panel_Histogram
            // 
            resources.ApplyResources(this.panel_Histogram, "panel_Histogram");
            this.panel_Histogram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.panel_Histogram.Controls.Add(this.pic_Histogram);
            this.panel_Histogram.Name = "panel_Histogram";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.chk_SetToOtherSideROIs);
            this.panel1.Controls.Add(this.btn_OK);
            this.panel1.Controls.Add(this.btn_Cancel);
            this.panel1.Name = "panel1";
            // 
            // chk_SetToOtherSideROIs
            // 
            resources.ApplyResources(this.chk_SetToOtherSideROIs, "chk_SetToOtherSideROIs");
            this.chk_SetToOtherSideROIs.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetToOtherSideROIs.Name = "chk_SetToOtherSideROIs";
            this.chk_SetToOtherSideROIs.Selected = false;
            this.chk_SetToOtherSideROIs.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToOtherSideROIs.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToOtherSideROIs.UseVisualStyleBackColor = true;
            this.chk_SetToOtherSideROIs.Click += new System.EventHandler(this.chk_SetToOtherSideROIs_Click);
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.pic_ROI);
            this.panel2.Name = "panel2";
            // 
            // pic_ROI
            // 
            resources.ApplyResources(this.pic_ROI, "pic_ROI");
            this.pic_ROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.pic_ROI.Name = "pic_ROI";
            this.pic_ROI.TabStop = false;
            // 
            // DoubleThresholdForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel_Histogram);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel_Gain);
            this.Controls.Add(this.panel_Threshold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DoubleThresholdForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DoubleThresholdForm_FormClosing);
            this.Load += new System.EventHandler(this.DoubleThresholdForm_Load);
            this.group_Threshold.ResumeLayout(false);
            this.group_Threshold.PerformLayout();
            this.pnl_Threshold.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_Histogram)).EndInit();
            this.panel_Threshold.ResumeLayout(false);
            this.panel_Gain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txt_ImageGain)).EndInit();
            this.panel_Histogram.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_ROI)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMGroupBox group_Threshold;
        private SRMControl.SRMInputBox txt_LowThreshold;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox txt_HighThreshold;
        private SRMControl.SRMLabel srmLabel2;
        private System.Windows.Forms.PictureBox pic_Histogram;
        private System.Windows.Forms.Panel pnl_Threshold;
        private System.Windows.Forms.Panel pnl_MiddleThreshold;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel pnl_HighThreshold;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel pnl_LowThreshold;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel_Threshold;
        private System.Windows.Forms.Panel panel_Gain;
        private System.Windows.Forms.Panel panel_Histogram;
        private System.Windows.Forms.NumericUpDown txt_ImageGain;
        private SRMControl.SRMLabel srmLabel26;
        private System.Windows.Forms.Panel panel1;
        private SRMControl.SRMCheckBox chk_ViewGrayImage;
        private SRMControl.SRMCheckBox chk_SetToOtherSideROIs;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pic_ROI;
    }
}