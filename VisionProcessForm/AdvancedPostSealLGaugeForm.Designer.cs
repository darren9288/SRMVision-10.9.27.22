namespace VisionProcessForm
{
    partial class AdvancedPostSealLGaugeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedPostSealLGaugeForm));
            this.panel_AdvanceSetting = new System.Windows.Forms.Panel();
            this.gb_AdvSetting = new System.Windows.Forms.GroupBox();
            this.txt_FilteringThreshold = new SRMControl.SRMInputBox();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.txt_MeasMinArea = new SRMControl.SRMInputBox();
            this.txt_FilteringPass = new SRMControl.SRMInputBox();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_MeasFilter = new SRMControl.SRMInputBox();
            this.panel_Setting = new System.Windows.Forms.Panel();
            this.trackBar_MinAmp = new System.Windows.Forms.TrackBar();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioBtn_LargeAmplitude = new SRMControl.SRMRadioButton();
            this.radioBtn_FromEnd = new SRMControl.SRMRadioButton();
            this.radioBtn_FromBegin = new SRMControl.SRMRadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioBtn_BW = new SRMControl.SRMRadioButton();
            this.radioBtn_WB = new SRMControl.SRMRadioButton();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.txt_MeasMinAmp = new SRMControl.SRMInputBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.txt_threshold = new SRMControl.SRMInputBox();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.txt_Tolerance = new SRMControl.SRMInputBox();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.txt_MeasThickness = new SRMControl.SRMInputBox();
            this.trackBar_Thickness = new System.Windows.Forms.TrackBar();
            this.trackBar_Derivative = new System.Windows.Forms.TrackBar();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.panel_SelectEdge = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pic_Down = new System.Windows.Forms.PictureBox();
            this.pic_Up = new System.Windows.Forms.PictureBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.ils_ImageListTree = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_ShowAdv = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.panel_AdvanceSetting.SuspendLayout();
            this.gb_AdvSetting.SuspendLayout();
            this.panel_Setting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_MinAmp)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Thickness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Derivative)).BeginInit();
            this.panel_SelectEdge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Down)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Up)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_AdvanceSetting
            // 
            resources.ApplyResources(this.panel_AdvanceSetting, "panel_AdvanceSetting");
            this.panel_AdvanceSetting.Controls.Add(this.gb_AdvSetting);
            this.panel_AdvanceSetting.Name = "panel_AdvanceSetting";
            // 
            // gb_AdvSetting
            // 
            resources.ApplyResources(this.gb_AdvSetting, "gb_AdvSetting");
            this.gb_AdvSetting.Controls.Add(this.txt_FilteringThreshold);
            this.gb_AdvSetting.Controls.Add(this.srmLabel11);
            this.gb_AdvSetting.Controls.Add(this.srmLabel4);
            this.gb_AdvSetting.Controls.Add(this.txt_MeasMinArea);
            this.gb_AdvSetting.Controls.Add(this.txt_FilteringPass);
            this.gb_AdvSetting.Controls.Add(this.srmLabel13);
            this.gb_AdvSetting.Controls.Add(this.srmLabel1);
            this.gb_AdvSetting.Controls.Add(this.txt_MeasFilter);
            this.gb_AdvSetting.Name = "gb_AdvSetting";
            this.gb_AdvSetting.TabStop = false;
            // 
            // txt_FilteringThreshold
            // 
            resources.ApplyResources(this.txt_FilteringThreshold, "txt_FilteringThreshold");
            this.txt_FilteringThreshold.BackColor = System.Drawing.Color.White;
            this.txt_FilteringThreshold.DataType = SRMControl.SRMDataType.Int32;
            this.txt_FilteringThreshold.DecimalPlaces = 1;
            this.txt_FilteringThreshold.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_FilteringThreshold.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_FilteringThreshold.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_FilteringThreshold.ForeColor = System.Drawing.Color.Black;
            this.txt_FilteringThreshold.InputType = SRMControl.InputType.Number;
            this.txt_FilteringThreshold.Name = "txt_FilteringThreshold";
            this.txt_FilteringThreshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_FilteringThreshold.TextChanged += new System.EventHandler(this.txt_FilteringThreshold_TextChanged);
            // 
            // srmLabel11
            // 
            resources.ApplyResources(this.srmLabel11, "srmLabel11");
            this.srmLabel11.Name = "srmLabel11";
            this.srmLabel11.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MeasMinArea
            // 
            resources.ApplyResources(this.txt_MeasMinArea, "txt_MeasMinArea");
            this.txt_MeasMinArea.BackColor = System.Drawing.Color.White;
            this.txt_MeasMinArea.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MeasMinArea.DecimalPlaces = 0;
            this.txt_MeasMinArea.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MeasMinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MeasMinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MeasMinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasMinArea.InputType = SRMControl.InputType.Number;
            this.txt_MeasMinArea.Name = "txt_MeasMinArea";
            this.txt_MeasMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasMinArea.TextChanged += new System.EventHandler(this.txt_MeasMinArea_TextChanged);
            // 
            // txt_FilteringPass
            // 
            resources.ApplyResources(this.txt_FilteringPass, "txt_FilteringPass");
            this.txt_FilteringPass.BackColor = System.Drawing.Color.White;
            this.txt_FilteringPass.DataType = SRMControl.SRMDataType.Int32;
            this.txt_FilteringPass.DecimalPlaces = 0;
            this.txt_FilteringPass.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_FilteringPass.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_FilteringPass.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_FilteringPass.ForeColor = System.Drawing.Color.Black;
            this.txt_FilteringPass.InputType = SRMControl.InputType.Number;
            this.txt_FilteringPass.Name = "txt_FilteringPass";
            this.txt_FilteringPass.NormalBackColor = System.Drawing.Color.White;
            this.txt_FilteringPass.TextChanged += new System.EventHandler(this.txt_FilteringPass_TextChanged);
            // 
            // srmLabel13
            // 
            resources.ApplyResources(this.srmLabel13, "srmLabel13");
            this.srmLabel13.Name = "srmLabel13";
            this.srmLabel13.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MeasFilter
            // 
            resources.ApplyResources(this.txt_MeasFilter, "txt_MeasFilter");
            this.txt_MeasFilter.BackColor = System.Drawing.Color.White;
            this.txt_MeasFilter.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MeasFilter.DecimalPlaces = 0;
            this.txt_MeasFilter.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MeasFilter.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MeasFilter.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MeasFilter.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasFilter.InputType = SRMControl.InputType.Number;
            this.txt_MeasFilter.Name = "txt_MeasFilter";
            this.txt_MeasFilter.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasFilter.TextChanged += new System.EventHandler(this.txt_MeasFilter_TextChanged);
            // 
            // panel_Setting
            // 
            resources.ApplyResources(this.panel_Setting, "panel_Setting");
            this.panel_Setting.Controls.Add(this.trackBar_MinAmp);
            this.panel_Setting.Controls.Add(this.groupBox3);
            this.panel_Setting.Controls.Add(this.groupBox2);
            this.panel_Setting.Controls.Add(this.srmLabel10);
            this.panel_Setting.Controls.Add(this.srmLabel2);
            this.panel_Setting.Controls.Add(this.txt_MeasMinAmp);
            this.panel_Setting.Controls.Add(this.srmLabel3);
            this.panel_Setting.Controls.Add(this.txt_threshold);
            this.panel_Setting.Controls.Add(this.srmLabel12);
            this.panel_Setting.Controls.Add(this.txt_Tolerance);
            this.panel_Setting.Controls.Add(this.srmLabel8);
            this.panel_Setting.Controls.Add(this.txt_MeasThickness);
            this.panel_Setting.Controls.Add(this.trackBar_Thickness);
            this.panel_Setting.Controls.Add(this.trackBar_Derivative);
            this.panel_Setting.Controls.Add(this.srmLabel5);
            this.panel_Setting.Name = "panel_Setting";
            // 
            // trackBar_MinAmp
            // 
            resources.ApplyResources(this.trackBar_MinAmp, "trackBar_MinAmp");
            this.trackBar_MinAmp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_MinAmp.LargeChange = 1;
            this.trackBar_MinAmp.Maximum = 100;
            this.trackBar_MinAmp.Name = "trackBar_MinAmp";
            this.trackBar_MinAmp.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_MinAmp.Value = 50;
            this.trackBar_MinAmp.Scroll += new System.EventHandler(this.trackBar_MinAmp_Scroll);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.radioBtn_LargeAmplitude);
            this.groupBox3.Controls.Add(this.radioBtn_FromEnd);
            this.groupBox3.Controls.Add(this.radioBtn_FromBegin);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioBtn_LargeAmplitude
            // 
            resources.ApplyResources(this.radioBtn_LargeAmplitude, "radioBtn_LargeAmplitude");
            this.radioBtn_LargeAmplitude.BackColor = System.Drawing.Color.Transparent;
            this.radioBtn_LargeAmplitude.Checked = true;
            this.radioBtn_LargeAmplitude.Name = "radioBtn_LargeAmplitude";
            this.radioBtn_LargeAmplitude.TabStop = true;
            this.radioBtn_LargeAmplitude.UseVisualStyleBackColor = false;
            this.radioBtn_LargeAmplitude.Click += new System.EventHandler(this.radioBtn_Search_Click);
            // 
            // radioBtn_FromEnd
            // 
            resources.ApplyResources(this.radioBtn_FromEnd, "radioBtn_FromEnd");
            this.radioBtn_FromEnd.BackColor = System.Drawing.Color.Transparent;
            this.radioBtn_FromEnd.Checked = true;
            this.radioBtn_FromEnd.Name = "radioBtn_FromEnd";
            this.radioBtn_FromEnd.TabStop = true;
            this.radioBtn_FromEnd.UseVisualStyleBackColor = false;
            this.radioBtn_FromEnd.Click += new System.EventHandler(this.radioBtn_Search_Click);
            // 
            // radioBtn_FromBegin
            // 
            resources.ApplyResources(this.radioBtn_FromBegin, "radioBtn_FromBegin");
            this.radioBtn_FromBegin.BackColor = System.Drawing.Color.Transparent;
            this.radioBtn_FromBegin.Checked = true;
            this.radioBtn_FromBegin.Name = "radioBtn_FromBegin";
            this.radioBtn_FromBegin.TabStop = true;
            this.radioBtn_FromBegin.UseVisualStyleBackColor = false;
            this.radioBtn_FromBegin.Click += new System.EventHandler(this.radioBtn_Search_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.radioBtn_BW);
            this.groupBox2.Controls.Add(this.radioBtn_WB);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioBtn_BW
            // 
            resources.ApplyResources(this.radioBtn_BW, "radioBtn_BW");
            this.radioBtn_BW.BackColor = System.Drawing.Color.Transparent;
            this.radioBtn_BW.Checked = true;
            this.radioBtn_BW.Name = "radioBtn_BW";
            this.radioBtn_BW.TabStop = true;
            this.radioBtn_BW.UseVisualStyleBackColor = false;
            this.radioBtn_BW.Click += new System.EventHandler(this.radioBtn_Polarity_Click);
            // 
            // radioBtn_WB
            // 
            resources.ApplyResources(this.radioBtn_WB, "radioBtn_WB");
            this.radioBtn_WB.BackColor = System.Drawing.Color.Transparent;
            this.radioBtn_WB.Checked = true;
            this.radioBtn_WB.Name = "radioBtn_WB";
            this.radioBtn_WB.TabStop = true;
            this.radioBtn_WB.UseVisualStyleBackColor = false;
            this.radioBtn_WB.Click += new System.EventHandler(this.radioBtn_Polarity_Click);
            // 
            // srmLabel10
            // 
            resources.ApplyResources(this.srmLabel10, "srmLabel10");
            this.srmLabel10.Name = "srmLabel10";
            this.srmLabel10.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MeasMinAmp
            // 
            resources.ApplyResources(this.txt_MeasMinAmp, "txt_MeasMinAmp");
            this.txt_MeasMinAmp.BackColor = System.Drawing.Color.White;
            this.txt_MeasMinAmp.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MeasMinAmp.DecimalPlaces = 0;
            this.txt_MeasMinAmp.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MeasMinAmp.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MeasMinAmp.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MeasMinAmp.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasMinAmp.InputType = SRMControl.InputType.Number;
            this.txt_MeasMinAmp.Name = "txt_MeasMinAmp";
            this.txt_MeasMinAmp.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasMinAmp.TextChanged += new System.EventHandler(this.txt_MeasMinAmp_TextChanged);
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_threshold
            // 
            resources.ApplyResources(this.txt_threshold, "txt_threshold");
            this.txt_threshold.BackColor = System.Drawing.Color.White;
            this.txt_threshold.DataType = SRMControl.SRMDataType.Int32;
            this.txt_threshold.DecimalPlaces = 0;
            this.txt_threshold.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_threshold.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_threshold.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_threshold.ForeColor = System.Drawing.Color.Black;
            this.txt_threshold.InputType = SRMControl.InputType.Number;
            this.txt_threshold.Name = "txt_threshold";
            this.txt_threshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_threshold.TextChanged += new System.EventHandler(this.txt_threshold_TextChanged);
            // 
            // srmLabel12
            // 
            resources.ApplyResources(this.srmLabel12, "srmLabel12");
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Tolerance
            // 
            resources.ApplyResources(this.txt_Tolerance, "txt_Tolerance");
            this.txt_Tolerance.BackColor = System.Drawing.Color.White;
            this.txt_Tolerance.DataType = SRMControl.SRMDataType.Int32;
            this.txt_Tolerance.DecimalPlaces = 0;
            this.txt_Tolerance.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_Tolerance.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Tolerance.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Tolerance.ForeColor = System.Drawing.Color.Black;
            this.txt_Tolerance.InputType = SRMControl.InputType.Number;
            this.txt_Tolerance.Name = "txt_Tolerance";
            this.txt_Tolerance.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MeasThickness
            // 
            resources.ApplyResources(this.txt_MeasThickness, "txt_MeasThickness");
            this.txt_MeasThickness.BackColor = System.Drawing.Color.White;
            this.txt_MeasThickness.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MeasThickness.DecimalPlaces = 0;
            this.txt_MeasThickness.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MeasThickness.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_MeasThickness.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MeasThickness.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasThickness.InputType = SRMControl.InputType.Number;
            this.txt_MeasThickness.Name = "txt_MeasThickness";
            this.txt_MeasThickness.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasThickness.TextChanged += new System.EventHandler(this.txt_MeasThickness_TextChanged);
            // 
            // trackBar_Thickness
            // 
            resources.ApplyResources(this.trackBar_Thickness, "trackBar_Thickness");
            this.trackBar_Thickness.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Thickness.LargeChange = 1;
            this.trackBar_Thickness.Maximum = 100;
            this.trackBar_Thickness.Minimum = 1;
            this.trackBar_Thickness.Name = "trackBar_Thickness";
            this.trackBar_Thickness.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Thickness.Value = 50;
            this.trackBar_Thickness.Scroll += new System.EventHandler(this.trackBar_Thickness_Scroll);
            // 
            // trackBar_Derivative
            // 
            resources.ApplyResources(this.trackBar_Derivative, "trackBar_Derivative");
            this.trackBar_Derivative.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Derivative.LargeChange = 1;
            this.trackBar_Derivative.Maximum = 100;
            this.trackBar_Derivative.Name = "trackBar_Derivative";
            this.trackBar_Derivative.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Derivative.Value = 50;
            this.trackBar_Derivative.Scroll += new System.EventHandler(this.trackBar_Derivative_Scroll);
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // panel_SelectEdge
            // 
            resources.ApplyResources(this.panel_SelectEdge, "panel_SelectEdge");
            this.panel_SelectEdge.Controls.Add(this.pictureBox1);
            this.panel_SelectEdge.Controls.Add(this.pic_Down);
            this.panel_SelectEdge.Controls.Add(this.pic_Up);
            this.panel_SelectEdge.Name = "panel_SelectEdge";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // pic_Down
            // 
            resources.ApplyResources(this.pic_Down, "pic_Down");
            this.pic_Down.BackColor = System.Drawing.Color.Black;
            this.pic_Down.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic_Down.Name = "pic_Down";
            this.pic_Down.TabStop = false;
            // 
            // pic_Up
            // 
            resources.ApplyResources(this.pic_Up, "pic_Up");
            this.pic_Up.BackColor = System.Drawing.Color.Black;
            this.pic_Up.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic_Up.Name = "pic_Up";
            this.pic_Up.TabStop = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "bw_ToDown.bmp");
            this.imageList1.Images.SetKeyName(1, "bw_ToLeft.bmp");
            this.imageList1.Images.SetKeyName(2, "bw_ToTop.bmp");
            this.imageList1.Images.SetKeyName(3, "bw_ToRight.bmp");
            this.imageList1.Images.SetKeyName(4, "wb_ToDown.bmp");
            this.imageList1.Images.SetKeyName(5, "wb_ToLeft.bmp");
            this.imageList1.Images.SetKeyName(6, "wb_ToTop.bmp");
            this.imageList1.Images.SetKeyName(7, "wb_ToRight.bmp");
            // 
            // Timer
            // 
            this.Timer.Enabled = true;
            // 
            // ils_ImageListTree
            // 
            this.ils_ImageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ils_ImageListTree.ImageStream")));
            this.ils_ImageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.ils_ImageListTree.Images.SetKeyName(0, "5S Center Gauge Unit ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(1, "5S Top Gauge Unit ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(2, "5S Right Gauge Unit ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(3, "5S Bottom Gauge Unit ROI.png");
            this.ils_ImageListTree.Images.SetKeyName(4, "5S Left Gauge Unit ROI.png");
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.btn_ShowAdv);
            this.panel1.Controls.Add(this.btn_OK);
            this.panel1.Controls.Add(this.btn_Cancel);
            this.panel1.Name = "panel1";
            // 
            // btn_ShowAdv
            // 
            resources.ApplyResources(this.btn_ShowAdv, "btn_ShowAdv");
            this.btn_ShowAdv.Name = "btn_ShowAdv";
            this.btn_ShowAdv.UseVisualStyleBackColor = true;
            this.btn_ShowAdv.Click += new System.EventHandler(this.btn_ShowAdv_Click);
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // AdvancedPostSealLGaugeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel_AdvanceSetting);
            this.Controls.Add(this.panel_Setting);
            this.Controls.Add(this.panel_SelectEdge);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedPostSealLGaugeForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdvancedPostSealLGaugeForm_FormClosing);
            this.Load += new System.EventHandler(this.AdvancedPostSealLGaugeForm_Load);
            this.panel_AdvanceSetting.ResumeLayout(false);
            this.gb_AdvSetting.ResumeLayout(false);
            this.gb_AdvSetting.PerformLayout();
            this.panel_Setting.ResumeLayout(false);
            this.panel_Setting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_MinAmp)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Thickness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Derivative)).EndInit();
            this.panel_SelectEdge.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Down)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Up)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_AdvanceSetting;
        private System.Windows.Forms.GroupBox gb_AdvSetting;
        private SRMControl.SRMInputBox txt_FilteringThreshold;
        private SRMControl.SRMLabel srmLabel11;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMInputBox txt_MeasMinArea;
        private SRMControl.SRMInputBox txt_FilteringPass;
        private SRMControl.SRMLabel srmLabel13;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox txt_MeasFilter;
        private System.Windows.Forms.Panel panel_Setting;
        private System.Windows.Forms.TrackBar trackBar_MinAmp;
        private System.Windows.Forms.GroupBox groupBox3;
        private SRMControl.SRMRadioButton radioBtn_LargeAmplitude;
        private SRMControl.SRMRadioButton radioBtn_FromEnd;
        private SRMControl.SRMRadioButton radioBtn_FromBegin;
        private System.Windows.Forms.GroupBox groupBox2;
        private SRMControl.SRMRadioButton radioBtn_BW;
        private SRMControl.SRMRadioButton radioBtn_WB;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMInputBox txt_MeasMinAmp;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMInputBox txt_threshold;
        private SRMControl.SRMLabel srmLabel12;
        private SRMControl.SRMInputBox txt_Tolerance;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMInputBox txt_MeasThickness;
        private System.Windows.Forms.TrackBar trackBar_Thickness;
        private System.Windows.Forms.TrackBar trackBar_Derivative;
        private SRMControl.SRMLabel srmLabel5;
        private System.Windows.Forms.Panel panel_SelectEdge;
        private System.Windows.Forms.PictureBox pic_Down;
        private System.Windows.Forms.PictureBox pic_Up;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.ImageList ils_ImageListTree;
        private System.Windows.Forms.Panel panel1;
        private SRMControl.SRMButton btn_ShowAdv;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMButton btn_Cancel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}