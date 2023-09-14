namespace VisionProcessForm
{
    partial class ColorThresholdForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorThresholdForm));
            this.lbl_Lightness = new SRMControl.SRMLabel();
            this.txt_Lightness = new SRMControl.SRMInputBox();
            this.lbl_Saturation = new SRMControl.SRMLabel();
            this.txt_Saturation = new SRMControl.SRMInputBox();
            this.lbl_Hue = new SRMControl.SRMLabel();
            this.trackBar_Hue = new System.Windows.Forms.TrackBar();
            this.txt_Hue = new SRMControl.SRMInputBox();
            this.group_LSHSetting = new SRMControl.SRMGroupBox();
            this.lbl_HueTolerance = new SRMControl.SRMLabel();
            this.lbl_SaturationTolerance = new SRMControl.SRMLabel();
            this.txt_SaturationTolerance = new System.Windows.Forms.NumericUpDown();
            this.txt_HueTolerance = new System.Windows.Forms.NumericUpDown();
            this.lbl_LightnessTolerance = new SRMControl.SRMLabel();
            this.txt_LightnessTolerance = new System.Windows.Forms.NumericUpDown();
            this.trackBar_Saturation = new System.Windows.Forms.TrackBar();
            this.pic_Saturation = new System.Windows.Forms.PictureBox();
            this.pic_Lightness = new System.Windows.Forms.PictureBox();
            this.pic_Hue = new System.Windows.Forms.PictureBox();
            this.trackBar_Lightness = new System.Windows.Forms.TrackBar();
            this.pnl_Threshold = new System.Windows.Forms.Panel();
            this.pnl_WhiteThreshold = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.pnl_HighThreshold = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pnl_LowThreshold = new System.Windows.Forms.Panel();
            this.txt_LowThreshold = new SRMControl.SRMInputBox();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.txt_HighThreshold = new SRMControl.SRMInputBox();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.panel4 = new System.Windows.Forms.Panel();
            this.srmLabel9 = new SRMControl.SRMLabel();
            this.srmInputBox3 = new SRMControl.SRMInputBox();
            this.srmInputBox4 = new SRMControl.SRMInputBox();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.splitter5 = new System.Windows.Forms.Splitter();
            this.panel7 = new System.Windows.Forms.Panel();
            this.splitter6 = new System.Windows.Forms.Splitter();
            this.panel8 = new System.Windows.Forms.Panel();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.srmInputBox5 = new SRMControl.SRMInputBox();
            this.srmInputBox6 = new SRMControl.SRMInputBox();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.chk_Preview = new SRMControl.SRMCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Hue)).BeginInit();
            this.group_LSHSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_SaturationTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_HueTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_LightnessTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Saturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Saturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Lightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Hue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Lightness)).BeginInit();
            this.pnl_Threshold.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_Lightness
            // 
            resources.ApplyResources(this.lbl_Lightness, "lbl_Lightness");
            this.lbl_Lightness.Name = "lbl_Lightness";
            this.lbl_Lightness.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Lightness
            // 
            this.txt_Lightness.BackColor = System.Drawing.Color.White;
            this.txt_Lightness.DecimalPlaces = 0;
            this.txt_Lightness.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Lightness.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Lightness.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Lightness.ForeColor = System.Drawing.Color.Black;
            this.txt_Lightness.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_Lightness, "txt_Lightness");
            this.txt_Lightness.Name = "txt_Lightness";
            this.txt_Lightness.NormalBackColor = System.Drawing.Color.White;
            this.txt_Lightness.TextChanged += new System.EventHandler(this.txt_Lightness_TextChanged);
            // 
            // lbl_Saturation
            // 
            resources.ApplyResources(this.lbl_Saturation, "lbl_Saturation");
            this.lbl_Saturation.Name = "lbl_Saturation";
            this.lbl_Saturation.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Saturation
            // 
            this.txt_Saturation.BackColor = System.Drawing.Color.White;
            this.txt_Saturation.DecimalPlaces = 0;
            this.txt_Saturation.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Saturation.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Saturation.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Saturation.ForeColor = System.Drawing.Color.Black;
            this.txt_Saturation.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_Saturation, "txt_Saturation");
            this.txt_Saturation.Name = "txt_Saturation";
            this.txt_Saturation.NormalBackColor = System.Drawing.Color.White;
            this.txt_Saturation.TextChanged += new System.EventHandler(this.txt_Saturation_TextChanged);
            // 
            // lbl_Hue
            // 
            resources.ApplyResources(this.lbl_Hue, "lbl_Hue");
            this.lbl_Hue.Name = "lbl_Hue";
            this.lbl_Hue.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // trackBar_Hue
            // 
            resources.ApplyResources(this.trackBar_Hue, "trackBar_Hue");
            this.trackBar_Hue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Hue.LargeChange = 1;
            this.trackBar_Hue.Maximum = 255;
            this.trackBar_Hue.Name = "trackBar_Hue";
            this.trackBar_Hue.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Hue.Value = 125;
            this.trackBar_Hue.Scroll += new System.EventHandler(this.trackBar_Hue_Scroll);
            // 
            // txt_Hue
            // 
            this.txt_Hue.BackColor = System.Drawing.Color.White;
            this.txt_Hue.DecimalPlaces = 0;
            this.txt_Hue.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Hue.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Hue.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Hue.ForeColor = System.Drawing.Color.Black;
            this.txt_Hue.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_Hue, "txt_Hue");
            this.txt_Hue.Name = "txt_Hue";
            this.txt_Hue.NormalBackColor = System.Drawing.Color.White;
            this.txt_Hue.TextChanged += new System.EventHandler(this.txt_Hue_TextChanged);
            // 
            // group_LSHSetting
            // 
            this.group_LSHSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_LSHSetting.Controls.Add(this.lbl_HueTolerance);
            this.group_LSHSetting.Controls.Add(this.lbl_SaturationTolerance);
            this.group_LSHSetting.Controls.Add(this.txt_SaturationTolerance);
            this.group_LSHSetting.Controls.Add(this.txt_HueTolerance);
            this.group_LSHSetting.Controls.Add(this.lbl_LightnessTolerance);
            this.group_LSHSetting.Controls.Add(this.txt_LightnessTolerance);
            this.group_LSHSetting.Controls.Add(this.lbl_Lightness);
            this.group_LSHSetting.Controls.Add(this.lbl_Saturation);
            this.group_LSHSetting.Controls.Add(this.txt_Saturation);
            this.group_LSHSetting.Controls.Add(this.txt_Lightness);
            this.group_LSHSetting.Controls.Add(this.trackBar_Saturation);
            this.group_LSHSetting.Controls.Add(this.pic_Saturation);
            this.group_LSHSetting.Controls.Add(this.pic_Lightness);
            this.group_LSHSetting.Controls.Add(this.pic_Hue);
            this.group_LSHSetting.Controls.Add(this.trackBar_Lightness);
            this.group_LSHSetting.Controls.Add(this.lbl_Hue);
            this.group_LSHSetting.Controls.Add(this.trackBar_Hue);
            this.group_LSHSetting.Controls.Add(this.txt_Hue);
            resources.ApplyResources(this.group_LSHSetting, "group_LSHSetting");
            this.group_LSHSetting.Name = "group_LSHSetting";
            this.group_LSHSetting.TabStop = false;
            // 
            // lbl_HueTolerance
            // 
            resources.ApplyResources(this.lbl_HueTolerance, "lbl_HueTolerance");
            this.lbl_HueTolerance.Name = "lbl_HueTolerance";
            this.lbl_HueTolerance.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_SaturationTolerance
            // 
            resources.ApplyResources(this.lbl_SaturationTolerance, "lbl_SaturationTolerance");
            this.lbl_SaturationTolerance.Name = "lbl_SaturationTolerance";
            this.lbl_SaturationTolerance.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_SaturationTolerance
            // 
            resources.ApplyResources(this.txt_SaturationTolerance, "txt_SaturationTolerance");
            this.txt_SaturationTolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_SaturationTolerance.Name = "txt_SaturationTolerance";
            this.txt_SaturationTolerance.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_SaturationTolerance.ValueChanged += new System.EventHandler(this.txt_SaturationTolerance_ValueChanged);
            // 
            // txt_HueTolerance
            // 
            resources.ApplyResources(this.txt_HueTolerance, "txt_HueTolerance");
            this.txt_HueTolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_HueTolerance.Name = "txt_HueTolerance";
            this.txt_HueTolerance.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_HueTolerance.ValueChanged += new System.EventHandler(this.txt_HueTolerance_ValueChanged);
            // 
            // lbl_LightnessTolerance
            // 
            resources.ApplyResources(this.lbl_LightnessTolerance, "lbl_LightnessTolerance");
            this.lbl_LightnessTolerance.Name = "lbl_LightnessTolerance";
            this.lbl_LightnessTolerance.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_LightnessTolerance
            // 
            resources.ApplyResources(this.txt_LightnessTolerance, "txt_LightnessTolerance");
            this.txt_LightnessTolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_LightnessTolerance.Name = "txt_LightnessTolerance";
            this.txt_LightnessTolerance.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_LightnessTolerance.ValueChanged += new System.EventHandler(this.txt_LightnessTolerance_ValueChanged);
            // 
            // trackBar_Saturation
            // 
            resources.ApplyResources(this.trackBar_Saturation, "trackBar_Saturation");
            this.trackBar_Saturation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Saturation.LargeChange = 1;
            this.trackBar_Saturation.Maximum = 255;
            this.trackBar_Saturation.Name = "trackBar_Saturation";
            this.trackBar_Saturation.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Saturation.Value = 125;
            this.trackBar_Saturation.Scroll += new System.EventHandler(this.trackBar_Saturation_Scroll);
            // 
            // pic_Saturation
            // 
            resources.ApplyResources(this.pic_Saturation, "pic_Saturation");
            this.pic_Saturation.Name = "pic_Saturation";
            this.pic_Saturation.TabStop = false;
            // 
            // pic_Lightness
            // 
            resources.ApplyResources(this.pic_Lightness, "pic_Lightness");
            this.pic_Lightness.Name = "pic_Lightness";
            this.pic_Lightness.TabStop = false;
            // 
            // pic_Hue
            // 
            resources.ApplyResources(this.pic_Hue, "pic_Hue");
            this.pic_Hue.Name = "pic_Hue";
            this.pic_Hue.TabStop = false;
            // 
            // trackBar_Lightness
            // 
            resources.ApplyResources(this.trackBar_Lightness, "trackBar_Lightness");
            this.trackBar_Lightness.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Lightness.LargeChange = 1;
            this.trackBar_Lightness.Maximum = 255;
            this.trackBar_Lightness.Name = "trackBar_Lightness";
            this.trackBar_Lightness.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Lightness.Value = 125;
            this.trackBar_Lightness.Scroll += new System.EventHandler(this.trackBar_Lightness_Scroll);
            // 
            // pnl_Threshold
            // 
            resources.ApplyResources(this.pnl_Threshold, "pnl_Threshold");
            this.pnl_Threshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_Threshold.Controls.Add(this.pnl_WhiteThreshold);
            this.pnl_Threshold.Controls.Add(this.splitter2);
            this.pnl_Threshold.Controls.Add(this.pnl_HighThreshold);
            this.pnl_Threshold.Controls.Add(this.splitter1);
            this.pnl_Threshold.Controls.Add(this.pnl_LowThreshold);
            this.pnl_Threshold.Name = "pnl_Threshold";
            // 
            // pnl_WhiteThreshold
            // 
            this.pnl_WhiteThreshold.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.pnl_WhiteThreshold, "pnl_WhiteThreshold");
            this.pnl_WhiteThreshold.Name = "pnl_WhiteThreshold";
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.Color.Silver;
            this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitter2, "splitter2");
            this.splitter2.Name = "splitter2";
            this.splitter2.TabStop = false;
            // 
            // pnl_HighThreshold
            // 
            this.pnl_HighThreshold.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.pnl_HighThreshold, "pnl_HighThreshold");
            this.pnl_HighThreshold.Name = "pnl_HighThreshold";
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.Color.Silver;
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // pnl_LowThreshold
            // 
            this.pnl_LowThreshold.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.pnl_LowThreshold, "pnl_LowThreshold");
            this.pnl_LowThreshold.Name = "pnl_LowThreshold";
            // 
            // txt_LowThreshold
            // 
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
            resources.ApplyResources(this.txt_LowThreshold, "txt_LowThreshold");
            this.txt_LowThreshold.Name = "txt_LowThreshold";
            this.txt_LowThreshold.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_HighThreshold
            // 
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
            resources.ApplyResources(this.txt_HighThreshold, "txt_HighThreshold");
            this.txt_HighThreshold.Name = "txt_HighThreshold";
            this.txt_HighThreshold.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel6
            // 
            resources.ApplyResources(this.srmLabel6, "srmLabel6");
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // pictureBox4
            // 
            resources.ApplyResources(this.pictureBox4, "pictureBox4");
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            resources.ApplyResources(this.pictureBox5, "pictureBox5");
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.TabStop = false;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.splitter3);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.splitter4);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // splitter3
            // 
            this.splitter3.BackColor = System.Drawing.Color.Silver;
            this.splitter3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitter3, "splitter3");
            this.splitter3.Name = "splitter3";
            this.splitter3.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // splitter4
            // 
            this.splitter4.BackColor = System.Drawing.Color.Silver;
            this.splitter4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitter4, "splitter4");
            this.splitter4.Name = "splitter4";
            this.splitter4.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // srmLabel9
            // 
            resources.ApplyResources(this.srmLabel9, "srmLabel9");
            this.srmLabel9.Name = "srmLabel9";
            this.srmLabel9.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmInputBox3
            // 
            this.srmInputBox3.BackColor = System.Drawing.Color.White;
            this.srmInputBox3.DecimalPlaces = 0;
            this.srmInputBox3.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.srmInputBox3.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.srmInputBox3.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.srmInputBox3.ForeColor = System.Drawing.Color.Black;
            this.srmInputBox3.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.srmInputBox3, "srmInputBox3");
            this.srmInputBox3.Name = "srmInputBox3";
            this.srmInputBox3.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmInputBox4
            // 
            this.srmInputBox4.BackColor = System.Drawing.Color.White;
            this.srmInputBox4.DecimalPlaces = 0;
            this.srmInputBox4.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.srmInputBox4.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.srmInputBox4.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.srmInputBox4.ForeColor = System.Drawing.Color.Black;
            this.srmInputBox4.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.srmInputBox4, "srmInputBox4");
            this.srmInputBox4.Name = "srmInputBox4";
            this.srmInputBox4.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel10
            // 
            resources.ApplyResources(this.srmLabel10, "srmLabel10");
            this.srmLabel10.Name = "srmLabel10";
            this.srmLabel10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel11
            // 
            resources.ApplyResources(this.srmLabel11, "srmLabel11");
            this.srmLabel11.Name = "srmLabel11";
            this.srmLabel11.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // panel5
            // 
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Controls.Add(this.splitter5);
            this.panel5.Controls.Add(this.panel7);
            this.panel5.Controls.Add(this.splitter6);
            this.panel5.Controls.Add(this.panel8);
            this.panel5.Name = "panel5";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // splitter5
            // 
            this.splitter5.BackColor = System.Drawing.Color.Silver;
            this.splitter5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitter5, "splitter5");
            this.splitter5.Name = "splitter5";
            this.splitter5.TabStop = false;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // splitter6
            // 
            this.splitter6.BackColor = System.Drawing.Color.Silver;
            this.splitter6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitter6, "splitter6");
            this.splitter6.Name = "splitter6";
            this.splitter6.TabStop = false;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.panel8, "panel8");
            this.panel8.Name = "panel8";
            // 
            // srmLabel12
            // 
            resources.ApplyResources(this.srmLabel12, "srmLabel12");
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmInputBox5
            // 
            this.srmInputBox5.BackColor = System.Drawing.Color.White;
            this.srmInputBox5.DecimalPlaces = 0;
            this.srmInputBox5.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.srmInputBox5.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.srmInputBox5.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.srmInputBox5.ForeColor = System.Drawing.Color.Black;
            this.srmInputBox5.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.srmInputBox5, "srmInputBox5");
            this.srmInputBox5.Name = "srmInputBox5";
            this.srmInputBox5.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmInputBox6
            // 
            this.srmInputBox6.BackColor = System.Drawing.Color.White;
            this.srmInputBox6.DecimalPlaces = 0;
            this.srmInputBox6.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.srmInputBox6.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.srmInputBox6.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.srmInputBox6.ForeColor = System.Drawing.Color.Black;
            this.srmInputBox6.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.srmInputBox6, "srmInputBox6");
            this.srmInputBox6.Name = "srmInputBox6";
            this.srmInputBox6.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel13
            // 
            resources.ApplyResources(this.srmLabel13, "srmLabel13");
            this.srmLabel13.Name = "srmLabel13";
            this.srmLabel13.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // chk_Preview
            // 
            this.chk_Preview.Checked = true;
            this.chk_Preview.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Preview.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.chk_Preview, "chk_Preview");
            this.chk_Preview.Name = "chk_Preview";
            this.chk_Preview.Selected = false;
            this.chk_Preview.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Preview.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Preview.UseVisualStyleBackColor = true;
            this.chk_Preview.Click += new System.EventHandler(this.chk_Preview_Click);
            // 
            // ColorThresholdForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.chk_Preview);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.srmLabel11);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.srmLabel12);
            this.Controls.Add(this.srmInputBox5);
            this.Controls.Add(this.srmInputBox6);
            this.Controls.Add(this.srmLabel13);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.srmLabel8);
            this.Controls.Add(this.group_LSHSetting);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.srmLabel9);
            this.Controls.Add(this.srmInputBox3);
            this.Controls.Add(this.srmInputBox4);
            this.Controls.Add(this.srmLabel10);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.srmLabel7);
            this.Controls.Add(this.srmLabel5);
            this.Controls.Add(this.txt_LowThreshold);
            this.Controls.Add(this.pnl_Threshold);
            this.Controls.Add(this.txt_HighThreshold);
            this.Controls.Add(this.srmLabel6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorThresholdForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ColorThresholdForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorThresholdForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Hue)).EndInit();
            this.group_LSHSetting.ResumeLayout(false);
            this.group_LSHSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_SaturationTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_HueTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_LightnessTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Saturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Saturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Lightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Hue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Lightness)).EndInit();
            this.pnl_Threshold.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMLabel lbl_Lightness;
        private SRMControl.SRMInputBox txt_Lightness;
        private SRMControl.SRMLabel lbl_Saturation;
        private SRMControl.SRMInputBox txt_Saturation;
        private SRMControl.SRMLabel lbl_Hue;
        private System.Windows.Forms.TrackBar trackBar_Hue;
        private SRMControl.SRMInputBox txt_Hue;
        private SRMControl.SRMGroupBox group_LSHSetting;
        private System.Windows.Forms.PictureBox pic_Hue;
        private System.Windows.Forms.PictureBox pic_Saturation;
        private System.Windows.Forms.TrackBar trackBar_Saturation;
        private System.Windows.Forms.PictureBox pic_Lightness;
        private System.Windows.Forms.TrackBar trackBar_Lightness;
        private System.Windows.Forms.Panel pnl_Threshold;
        private System.Windows.Forms.Panel pnl_WhiteThreshold;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel pnl_HighThreshold;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel pnl_LowThreshold;
        private SRMControl.SRMInputBox txt_LowThreshold;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMInputBox txt_HighThreshold;
        private SRMControl.SRMLabel srmLabel6;
        private SRMControl.SRMLabel srmLabel7;
        private System.Windows.Forms.PictureBox pictureBox4;
        private SRMControl.SRMLabel srmLabel11;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Splitter splitter5;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Splitter splitter6;
        private System.Windows.Forms.Panel panel8;
        private SRMControl.SRMLabel srmLabel12;
        private SRMControl.SRMInputBox srmInputBox5;
        private SRMControl.SRMInputBox srmInputBox6;
        private SRMControl.SRMLabel srmLabel13;
        private System.Windows.Forms.PictureBox pictureBox5;
        private SRMControl.SRMLabel srmLabel8;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.Panel panel4;
        private SRMControl.SRMLabel srmLabel9;
        private SRMControl.SRMInputBox srmInputBox3;
        private SRMControl.SRMInputBox srmInputBox4;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.NumericUpDown txt_LightnessTolerance;
        private SRMControl.SRMLabel lbl_LightnessTolerance;
        private SRMControl.SRMLabel lbl_HueTolerance;
        private System.Windows.Forms.NumericUpDown txt_HueTolerance;
        private SRMControl.SRMLabel lbl_SaturationTolerance;
        private System.Windows.Forms.NumericUpDown txt_SaturationTolerance;
        private System.Windows.Forms.Timer timer;
        private SRMControl.SRMCheckBox chk_Preview;

    }
}