namespace VisionProcessForm
{
    partial class ThresholdForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThresholdForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.group_Threshold = new SRMControl.SRMGroupBox();
            this.pnl_Relative = new System.Windows.Forms.Panel();
            this.txt_Relative = new System.Windows.Forms.NumericUpDown();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.chk_SetToAllTemplate = new SRMControl.SRMCheckBox();
            this.lbl_PixelDetail = new SRMControl.SRMLabel();
            this.lbl_Transition = new SRMControl.SRMLabel();
            this.cbo_AreaColor = new SRMControl.SRMComboBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.txt_CountAreaPixel = new SRMControl.SRMInputBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_ThresholdValue = new SRMControl.SRMInputBox();
            this.chk_MinResidue = new SRMControl.SRMCheckBox();
            this.trackBar_Threshold = new System.Windows.Forms.TrackBar();
            this.lbl_Threshold = new SRMControl.SRMLabel();
            this.pic_Histogram = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chk_SetToOtherSideROIs = new SRMControl.SRMCheckBox();
            this.pic_ROI = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.group_Threshold.SuspendLayout();
            this.pnl_Relative.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Relative)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Histogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ROI)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
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
            // 
            // group_Threshold
            // 
            resources.ApplyResources(this.group_Threshold, "group_Threshold");
            this.group_Threshold.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_Threshold.Controls.Add(this.pnl_Relative);
            this.group_Threshold.Controls.Add(this.chk_SetToAllTemplate);
            this.group_Threshold.Controls.Add(this.lbl_PixelDetail);
            this.group_Threshold.Controls.Add(this.lbl_Transition);
            this.group_Threshold.Controls.Add(this.cbo_AreaColor);
            this.group_Threshold.Controls.Add(this.srmLabel3);
            this.group_Threshold.Controls.Add(this.txt_CountAreaPixel);
            this.group_Threshold.Controls.Add(this.srmLabel1);
            this.group_Threshold.Controls.Add(this.txt_ThresholdValue);
            this.group_Threshold.Controls.Add(this.chk_MinResidue);
            this.group_Threshold.Controls.Add(this.trackBar_Threshold);
            this.group_Threshold.Controls.Add(this.lbl_Threshold);
            this.group_Threshold.Name = "group_Threshold";
            this.group_Threshold.TabStop = false;
            // 
            // pnl_Relative
            // 
            resources.ApplyResources(this.pnl_Relative, "pnl_Relative");
            this.pnl_Relative.Controls.Add(this.txt_Relative);
            this.pnl_Relative.Controls.Add(this.srmLabel5);
            this.pnl_Relative.Controls.Add(this.srmLabel4);
            this.pnl_Relative.Name = "pnl_Relative";
            // 
            // txt_Relative
            // 
            resources.ApplyResources(this.txt_Relative, "txt_Relative");
            this.txt_Relative.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.txt_Relative.Name = "txt_Relative";
            this.txt_Relative.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.txt_Relative.ValueChanged += new System.EventHandler(this.txt_Relative_ValueChanged);
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // chk_SetToAllTemplate
            // 
            resources.ApplyResources(this.chk_SetToAllTemplate, "chk_SetToAllTemplate");
            this.chk_SetToAllTemplate.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_SetToAllTemplate.Name = "chk_SetToAllTemplate";
            this.chk_SetToAllTemplate.Selected = false;
            this.chk_SetToAllTemplate.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_SetToAllTemplate.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_SetToAllTemplate.UseVisualStyleBackColor = true;
            // 
            // lbl_PixelDetail
            // 
            resources.ApplyResources(this.lbl_PixelDetail, "lbl_PixelDetail");
            this.lbl_PixelDetail.ForeColor = System.Drawing.Color.Red;
            this.lbl_PixelDetail.Name = "lbl_PixelDetail";
            this.lbl_PixelDetail.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Transition
            // 
            resources.ApplyResources(this.lbl_Transition, "lbl_Transition");
            this.lbl_Transition.ForeColor = System.Drawing.Color.Red;
            this.lbl_Transition.Name = "lbl_Transition";
            this.lbl_Transition.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_AreaColor
            // 
            resources.ApplyResources(this.cbo_AreaColor, "cbo_AreaColor");
            this.cbo_AreaColor.BackColor = System.Drawing.Color.White;
            this.cbo_AreaColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_AreaColor.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_AreaColor.FormattingEnabled = true;
            this.cbo_AreaColor.Items.AddRange(new object[] {
            resources.GetString("cbo_AreaColor.Items"),
            resources.GetString("cbo_AreaColor.Items1")});
            this.cbo_AreaColor.Name = "cbo_AreaColor";
            this.cbo_AreaColor.NormalBackColor = System.Drawing.Color.White;
            this.cbo_AreaColor.SelectedIndexChanged += new System.EventHandler(this.cbo_AreaColor_SelectedIndexChanged);
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_CountAreaPixel
            // 
            resources.ApplyResources(this.txt_CountAreaPixel, "txt_CountAreaPixel");
            this.txt_CountAreaPixel.BackColor = System.Drawing.Color.White;
            this.txt_CountAreaPixel.DecimalPlaces = 0;
            this.txt_CountAreaPixel.DecMaxValue = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.txt_CountAreaPixel.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_CountAreaPixel.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_CountAreaPixel.ForeColor = System.Drawing.Color.Black;
            this.txt_CountAreaPixel.InputType = SRMControl.InputType.Number;
            this.txt_CountAreaPixel.Name = "txt_CountAreaPixel";
            this.txt_CountAreaPixel.NormalBackColor = System.Drawing.Color.White;
            this.txt_CountAreaPixel.TextChanged += new System.EventHandler(this.txt_CountAreaPixel_TextChanged);
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_ThresholdValue
            // 
            resources.ApplyResources(this.txt_ThresholdValue, "txt_ThresholdValue");
            this.txt_ThresholdValue.BackColor = System.Drawing.Color.White;
            this.txt_ThresholdValue.DecimalPlaces = 0;
            this.txt_ThresholdValue.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_ThresholdValue.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ThresholdValue.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ThresholdValue.ForeColor = System.Drawing.Color.Black;
            this.txt_ThresholdValue.InputType = SRMControl.InputType.Number;
            this.txt_ThresholdValue.Name = "txt_ThresholdValue";
            this.txt_ThresholdValue.NormalBackColor = System.Drawing.Color.White;
            this.txt_ThresholdValue.TextChanged += new System.EventHandler(this.txt_ThresholdValue_TextChanged);
            // 
            // chk_MinResidue
            // 
            resources.ApplyResources(this.chk_MinResidue, "chk_MinResidue");
            this.chk_MinResidue.Checked = true;
            this.chk_MinResidue.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_MinResidue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_MinResidue.Name = "chk_MinResidue";
            this.chk_MinResidue.Selected = false;
            this.chk_MinResidue.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_MinResidue.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_MinResidue.UseVisualStyleBackColor = true;
            this.chk_MinResidue.CheckedChanged += new System.EventHandler(this.chk_MinResidue_CheckedChanged);
            // 
            // trackBar_Threshold
            // 
            resources.ApplyResources(this.trackBar_Threshold, "trackBar_Threshold");
            this.trackBar_Threshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Threshold.LargeChange = 1;
            this.trackBar_Threshold.Maximum = 255;
            this.trackBar_Threshold.Name = "trackBar_Threshold";
            this.trackBar_Threshold.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Threshold.Value = 125;
            this.trackBar_Threshold.Scroll += new System.EventHandler(this.trackBar_Threshold_Scroll);
            // 
            // lbl_Threshold
            // 
            resources.ApplyResources(this.lbl_Threshold, "lbl_Threshold");
            this.lbl_Threshold.Name = "lbl_Threshold";
            this.lbl_Threshold.TextShadowColor = System.Drawing.Color.Gray;
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
            // pic_ROI
            // 
            resources.ApplyResources(this.pic_ROI, "pic_ROI");
            this.pic_ROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.pic_ROI.Name = "pic_ROI";
            this.pic_ROI.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.pic_ROI);
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.chk_SetToOtherSideROIs);
            this.panel2.Controls.Add(this.btn_OK);
            this.panel2.Controls.Add(this.btn_Cancel);
            this.panel2.Name = "panel2";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.group_Threshold);
            this.panel3.Name = "panel3";
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Controls.Add(this.pic_Histogram);
            this.panel4.Name = "panel4";
            // 
            // ThresholdForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ThresholdForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ThresholdForm_FormClosing);
            this.Load += new System.EventHandler(this.ThresholdForm_Load);
            this.group_Threshold.ResumeLayout(false);
            this.group_Threshold.PerformLayout();
            this.pnl_Relative.ResumeLayout(false);
            this.pnl_Relative.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Relative)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Histogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ROI)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMGroupBox group_Threshold;
        private SRMControl.SRMInputBox txt_ThresholdValue;
        private SRMControl.SRMCheckBox chk_MinResidue;
        private SRMControl.SRMLabel lbl_Threshold;
        private System.Windows.Forms.TrackBar trackBar_Threshold;
        private System.Windows.Forms.PictureBox pic_Histogram;
        private System.Windows.Forms.Timer timer1;
        private SRMControl.SRMInputBox txt_CountAreaPixel;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMComboBox cbo_AreaColor;
        private SRMControl.SRMLabel lbl_Transition;
        private SRMControl.SRMLabel lbl_PixelDetail;
        private SRMControl.SRMCheckBox chk_SetToOtherSideROIs;
        private System.Windows.Forms.PictureBox pic_ROI;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private SRMControl.SRMCheckBox chk_SetToAllTemplate;
        private System.Windows.Forms.Panel pnl_Relative;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMLabel srmLabel4;
        private System.Windows.Forms.NumericUpDown txt_Relative;
    }
}