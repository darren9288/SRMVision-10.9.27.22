namespace VisionProcessForm
{
    partial class CalibrationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibrationForm));
            this.srmTabControl1 = new SRMControl.SRMTabControl();
            this.tabPage_Calibration = new System.Windows.Forms.TabPage();
            this.btn_Threshold = new SRMControl.SRMButton();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_MaxArea = new SRMControl.SRMInputBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_MinArea = new SRMControl.SRMInputBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_GridPitchY = new SRMControl.SRMInputBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_GridPitchX = new SRMControl.SRMInputBox();
            this.tabPage_CalibrationByCircle = new System.Windows.Forms.TabPage();
            this.chk_ShowDraggingBox = new SRMControl.SRMCheckBox();
            this.chk_ShowSamplePoints = new SRMControl.SRMCheckBox();
            this.txt_GaugeThreshold = new SRMControl.SRMInputBox();
            this.label79 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.txt_GaugeThickness = new SRMControl.SRMInputBox();
            this.txt_Diameter = new SRMControl.SRMInputBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label78 = new System.Windows.Forms.Label();
            this.cbo_TransitionType = new System.Windows.Forms.ComboBox();
            this.cbo_TransitionChoice = new System.Windows.Forms.ComboBox();
            this.tabPage_CalibrationByXY = new System.Windows.Forms.TabPage();
            this.btn_GaugeAdvanceSetting = new SRMControl.SRMButton();
            this.chk_UseGauge1 = new SRMControl.SRMCheckBox();
            this.txt_YDistance = new SRMControl.SRMInputBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_XDistance = new SRMControl.SRMInputBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPage_Calibration4S = new System.Windows.Forms.TabPage();
            this.chk_Rotate90Deg = new SRMControl.SRMCheckBox();
            this.chk_UseGauge = new SRMControl.SRMCheckBox();
            this.txt_ZDistance = new SRMControl.SRMInputBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tp_Offset = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_Set = new SRMControl.SRMButton();
            this.label29 = new System.Windows.Forms.Label();
            this.txt_CalTol = new SRMControl.SRMInputBox();
            this.label30 = new System.Windows.Forms.Label();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.btn_ApplyCurrentValue = new SRMControl.SRMButton();
            this.label19 = new System.Windows.Forms.Label();
            this.txt_ZOffSet = new SRMControl.SRMInputBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_HeightOffSet = new SRMControl.SRMInputBox();
            this.txt_WidthOffSet = new SRMControl.SRMInputBox();
            this.label76 = new System.Windows.Forms.Label();
            this.label77 = new System.Windows.Forms.Label();
            this.btn_AdvancedSettings = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_Calibrate = new SRMControl.SRMButton();
            this.gb_Result = new System.Windows.Forms.GroupBox();
            this.lbl_FOV = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.txt_FOV = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.txt_MeasuredZ = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.txt_MeasuredY = new System.Windows.Forms.TextBox();
            this.txt_MeasuredX = new System.Windows.Forms.TextBox();
            this.radioBtn_mmperPixel = new SRMControl.SRMRadioButton();
            this.radioBtn_pixelpermm = new SRMControl.SRMRadioButton();
            this.label24 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.txt_ZResolution = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.txt_YResolution = new System.Windows.Forms.TextBox();
            this.txt_XResolution = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txt_PixelSizeY = new System.Windows.Forms.TextBox();
            this.txt_PixelSizeX = new System.Windows.Forms.TextBox();
            this.lbl_ResultStatus = new System.Windows.Forms.Label();
            this.btn_Camera = new SRMControl.SRMButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.srmTabControl1.SuspendLayout();
            this.tabPage_Calibration.SuspendLayout();
            this.tabPage_CalibrationByCircle.SuspendLayout();
            this.tabPage_CalibrationByXY.SuspendLayout();
            this.tabPage_Calibration4S.SuspendLayout();
            this.tp_Offset.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.gb_Result.SuspendLayout();
            this.SuspendLayout();
            // 
            // srmTabControl1
            // 
            resources.ApplyResources(this.srmTabControl1, "srmTabControl1");
            this.srmTabControl1.Controls.Add(this.tabPage_Calibration);
            this.srmTabControl1.Controls.Add(this.tabPage_CalibrationByCircle);
            this.srmTabControl1.Controls.Add(this.tabPage_CalibrationByXY);
            this.srmTabControl1.Controls.Add(this.tabPage_Calibration4S);
            this.srmTabControl1.Controls.Add(this.tp_Offset);
            this.srmTabControl1.Name = "srmTabControl1";
            this.srmTabControl1.SelectedIndex = 0;
            this.srmTabControl1.SelectedIndexChanged += new System.EventHandler(this.srmTabControl1_SelectedIndexChanged);
            // 
            // tabPage_Calibration
            // 
            resources.ApplyResources(this.tabPage_Calibration, "tabPage_Calibration");
            this.tabPage_Calibration.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tabPage_Calibration.Controls.Add(this.btn_Threshold);
            this.tabPage_Calibration.Controls.Add(this.label4);
            this.tabPage_Calibration.Controls.Add(this.txt_MaxArea);
            this.tabPage_Calibration.Controls.Add(this.label3);
            this.tabPage_Calibration.Controls.Add(this.txt_MinArea);
            this.tabPage_Calibration.Controls.Add(this.label2);
            this.tabPage_Calibration.Controls.Add(this.txt_GridPitchY);
            this.tabPage_Calibration.Controls.Add(this.label5);
            this.tabPage_Calibration.Controls.Add(this.txt_GridPitchX);
            this.tabPage_Calibration.Name = "tabPage_Calibration";
            // 
            // btn_Threshold
            // 
            resources.ApplyResources(this.btn_Threshold, "btn_Threshold");
            this.btn_Threshold.Name = "btn_Threshold";
            this.btn_Threshold.UseVisualStyleBackColor = true;
            this.btn_Threshold.Click += new System.EventHandler(this.btn_Threshold_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txt_MaxArea
            // 
            resources.ApplyResources(this.txt_MaxArea, "txt_MaxArea");
            this.txt_MaxArea.BackColor = System.Drawing.Color.White;
            this.txt_MaxArea.DecimalPlaces = 0;
            this.txt_MaxArea.DecMaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_MaxArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MaxArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MaxArea.ForeColor = System.Drawing.Color.Black;
            this.txt_MaxArea.InputType = SRMControl.InputType.Number;
            this.txt_MaxArea.Name = "txt_MaxArea";
            this.txt_MaxArea.NormalBackColor = System.Drawing.Color.White;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txt_MinArea
            // 
            resources.ApplyResources(this.txt_MinArea, "txt_MinArea");
            this.txt_MinArea.BackColor = System.Drawing.Color.White;
            this.txt_MinArea.DecimalPlaces = 0;
            this.txt_MinArea.DecMaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_MinArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MinArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MinArea.ForeColor = System.Drawing.Color.Black;
            this.txt_MinArea.InputType = SRMControl.InputType.Number;
            this.txt_MinArea.Name = "txt_MinArea";
            this.txt_MinArea.NormalBackColor = System.Drawing.Color.White;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txt_GridPitchY
            // 
            resources.ApplyResources(this.txt_GridPitchY, "txt_GridPitchY");
            this.txt_GridPitchY.BackColor = System.Drawing.Color.White;
            this.txt_GridPitchY.DecimalPlaces = 2;
            this.txt_GridPitchY.DecMaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_GridPitchY.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_GridPitchY.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_GridPitchY.ForeColor = System.Drawing.Color.Black;
            this.txt_GridPitchY.InputType = SRMControl.InputType.Number;
            this.txt_GridPitchY.Name = "txt_GridPitchY";
            this.txt_GridPitchY.NormalBackColor = System.Drawing.Color.White;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // txt_GridPitchX
            // 
            resources.ApplyResources(this.txt_GridPitchX, "txt_GridPitchX");
            this.txt_GridPitchX.BackColor = System.Drawing.Color.White;
            this.txt_GridPitchX.DecimalPlaces = 2;
            this.txt_GridPitchX.DecMaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_GridPitchX.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_GridPitchX.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_GridPitchX.ForeColor = System.Drawing.Color.Black;
            this.txt_GridPitchX.InputType = SRMControl.InputType.Number;
            this.txt_GridPitchX.Name = "txt_GridPitchX";
            this.txt_GridPitchX.NormalBackColor = System.Drawing.Color.White;
            // 
            // tabPage_CalibrationByCircle
            // 
            resources.ApplyResources(this.tabPage_CalibrationByCircle, "tabPage_CalibrationByCircle");
            this.tabPage_CalibrationByCircle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tabPage_CalibrationByCircle.Controls.Add(this.chk_ShowDraggingBox);
            this.tabPage_CalibrationByCircle.Controls.Add(this.chk_ShowSamplePoints);
            this.tabPage_CalibrationByCircle.Controls.Add(this.txt_GaugeThreshold);
            this.tabPage_CalibrationByCircle.Controls.Add(this.label79);
            this.tabPage_CalibrationByCircle.Controls.Add(this.label8);
            this.tabPage_CalibrationByCircle.Controls.Add(this.label56);
            this.tabPage_CalibrationByCircle.Controls.Add(this.txt_GaugeThickness);
            this.tabPage_CalibrationByCircle.Controls.Add(this.txt_Diameter);
            this.tabPage_CalibrationByCircle.Controls.Add(this.label7);
            this.tabPage_CalibrationByCircle.Controls.Add(this.label78);
            this.tabPage_CalibrationByCircle.Controls.Add(this.cbo_TransitionType);
            this.tabPage_CalibrationByCircle.Controls.Add(this.cbo_TransitionChoice);
            this.tabPage_CalibrationByCircle.Name = "tabPage_CalibrationByCircle";
            // 
            // chk_ShowDraggingBox
            // 
            resources.ApplyResources(this.chk_ShowDraggingBox, "chk_ShowDraggingBox");
            this.chk_ShowDraggingBox.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_ShowDraggingBox.Name = "chk_ShowDraggingBox";
            this.chk_ShowDraggingBox.Selected = true;
            this.chk_ShowDraggingBox.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_ShowDraggingBox.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_ShowDraggingBox.UseVisualStyleBackColor = true;
            this.chk_ShowDraggingBox.Click += new System.EventHandler(this.chk_ShowDraggingBox_Click);
            // 
            // chk_ShowSamplePoints
            // 
            resources.ApplyResources(this.chk_ShowSamplePoints, "chk_ShowSamplePoints");
            this.chk_ShowSamplePoints.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_ShowSamplePoints.Name = "chk_ShowSamplePoints";
            this.chk_ShowSamplePoints.Selected = true;
            this.chk_ShowSamplePoints.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_ShowSamplePoints.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_ShowSamplePoints.UseVisualStyleBackColor = true;
            this.chk_ShowSamplePoints.Click += new System.EventHandler(this.chk_ShowSamplePoints_Click);
            // 
            // txt_GaugeThreshold
            // 
            resources.ApplyResources(this.txt_GaugeThreshold, "txt_GaugeThreshold");
            this.txt_GaugeThreshold.BackColor = System.Drawing.Color.White;
            this.txt_GaugeThreshold.DecimalPlaces = 0;
            this.txt_GaugeThreshold.DecMaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_GaugeThreshold.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_GaugeThreshold.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_GaugeThreshold.ForeColor = System.Drawing.Color.Black;
            this.txt_GaugeThreshold.InputType = SRMControl.InputType.Number;
            this.txt_GaugeThreshold.Name = "txt_GaugeThreshold";
            this.txt_GaugeThreshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_GaugeThreshold.TextChanged += new System.EventHandler(this.txt_GaugeThreshold_TextChanged);
            // 
            // label79
            // 
            resources.ApplyResources(this.label79, "label79");
            this.label79.Name = "label79";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label56
            // 
            resources.ApplyResources(this.label56, "label56");
            this.label56.Name = "label56";
            this.label56.Click += new System.EventHandler(this.label56_Click);
            // 
            // txt_GaugeThickness
            // 
            resources.ApplyResources(this.txt_GaugeThickness, "txt_GaugeThickness");
            this.txt_GaugeThickness.BackColor = System.Drawing.Color.White;
            this.txt_GaugeThickness.DecimalPlaces = 0;
            this.txt_GaugeThickness.DecMaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_GaugeThickness.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_GaugeThickness.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_GaugeThickness.ForeColor = System.Drawing.Color.Black;
            this.txt_GaugeThickness.InputType = SRMControl.InputType.Number;
            this.txt_GaugeThickness.Name = "txt_GaugeThickness";
            this.txt_GaugeThickness.NormalBackColor = System.Drawing.Color.White;
            this.txt_GaugeThickness.TextChanged += new System.EventHandler(this.txt_GaugeThickness_TextChanged);
            // 
            // txt_Diameter
            // 
            resources.ApplyResources(this.txt_Diameter, "txt_Diameter");
            this.txt_Diameter.BackColor = System.Drawing.Color.White;
            this.txt_Diameter.DecimalPlaces = 6;
            this.txt_Diameter.DecMaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_Diameter.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Diameter.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Diameter.ForeColor = System.Drawing.Color.Black;
            this.txt_Diameter.InputType = SRMControl.InputType.Number;
            this.txt_Diameter.Name = "txt_Diameter";
            this.txt_Diameter.NormalBackColor = System.Drawing.Color.White;
            this.txt_Diameter.TextChanged += new System.EventHandler(this.txt_Diameter_TextChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label78
            // 
            resources.ApplyResources(this.label78, "label78");
            this.label78.Name = "label78";
            // 
            // cbo_TransitionType
            // 
            resources.ApplyResources(this.cbo_TransitionType, "cbo_TransitionType");
            this.cbo_TransitionType.FormattingEnabled = true;
            this.cbo_TransitionType.Items.AddRange(new object[] {
            resources.GetString("cbo_TransitionType.Items"),
            resources.GetString("cbo_TransitionType.Items1")});
            this.cbo_TransitionType.Name = "cbo_TransitionType";
            this.cbo_TransitionType.SelectedIndexChanged += new System.EventHandler(this.cbo_TransitionType_SelectedIndexChanged);
            // 
            // cbo_TransitionChoice
            // 
            resources.ApplyResources(this.cbo_TransitionChoice, "cbo_TransitionChoice");
            this.cbo_TransitionChoice.FormattingEnabled = true;
            this.cbo_TransitionChoice.Items.AddRange(new object[] {
            resources.GetString("cbo_TransitionChoice.Items"),
            resources.GetString("cbo_TransitionChoice.Items1")});
            this.cbo_TransitionChoice.Name = "cbo_TransitionChoice";
            this.cbo_TransitionChoice.SelectedIndexChanged += new System.EventHandler(this.cbo_TransitionChoice_SelectedIndexChanged);
            // 
            // tabPage_CalibrationByXY
            // 
            resources.ApplyResources(this.tabPage_CalibrationByXY, "tabPage_CalibrationByXY");
            this.tabPage_CalibrationByXY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tabPage_CalibrationByXY.Controls.Add(this.btn_GaugeAdvanceSetting);
            this.tabPage_CalibrationByXY.Controls.Add(this.chk_UseGauge1);
            this.tabPage_CalibrationByXY.Controls.Add(this.txt_YDistance);
            this.tabPage_CalibrationByXY.Controls.Add(this.label9);
            this.tabPage_CalibrationByXY.Controls.Add(this.txt_XDistance);
            this.tabPage_CalibrationByXY.Controls.Add(this.label10);
            this.tabPage_CalibrationByXY.Name = "tabPage_CalibrationByXY";
            // 
            // btn_GaugeAdvanceSetting
            // 
            resources.ApplyResources(this.btn_GaugeAdvanceSetting, "btn_GaugeAdvanceSetting");
            this.btn_GaugeAdvanceSetting.Name = "btn_GaugeAdvanceSetting";
            this.btn_GaugeAdvanceSetting.UseVisualStyleBackColor = true;
            this.btn_GaugeAdvanceSetting.Click += new System.EventHandler(this.btn_GaugeAdvanceSetting_Click);
            // 
            // chk_UseGauge1
            // 
            resources.ApplyResources(this.chk_UseGauge1, "chk_UseGauge1");
            this.chk_UseGauge1.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_UseGauge1.Name = "chk_UseGauge1";
            this.chk_UseGauge1.Selected = true;
            this.chk_UseGauge1.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_UseGauge1.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_UseGauge1.UseVisualStyleBackColor = true;
            this.chk_UseGauge1.Click += new System.EventHandler(this.chk_UseGauge1_Click);
            // 
            // txt_YDistance
            // 
            resources.ApplyResources(this.txt_YDistance, "txt_YDistance");
            this.txt_YDistance.BackColor = System.Drawing.Color.White;
            this.txt_YDistance.DecimalPlaces = 3;
            this.txt_YDistance.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_YDistance.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_YDistance.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_YDistance.ForeColor = System.Drawing.Color.Black;
            this.txt_YDistance.InputType = SRMControl.InputType.Number;
            this.txt_YDistance.Name = "txt_YDistance";
            this.txt_YDistance.NormalBackColor = System.Drawing.Color.White;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // txt_XDistance
            // 
            resources.ApplyResources(this.txt_XDistance, "txt_XDistance");
            this.txt_XDistance.BackColor = System.Drawing.Color.White;
            this.txt_XDistance.DecimalPlaces = 3;
            this.txt_XDistance.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_XDistance.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_XDistance.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_XDistance.ForeColor = System.Drawing.Color.Black;
            this.txt_XDistance.InputType = SRMControl.InputType.Number;
            this.txt_XDistance.Name = "txt_XDistance";
            this.txt_XDistance.NormalBackColor = System.Drawing.Color.White;
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // tabPage_Calibration4S
            // 
            resources.ApplyResources(this.tabPage_Calibration4S, "tabPage_Calibration4S");
            this.tabPage_Calibration4S.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tabPage_Calibration4S.Controls.Add(this.chk_Rotate90Deg);
            this.tabPage_Calibration4S.Controls.Add(this.chk_UseGauge);
            this.tabPage_Calibration4S.Controls.Add(this.txt_ZDistance);
            this.tabPage_Calibration4S.Controls.Add(this.label18);
            this.tabPage_Calibration4S.Name = "tabPage_Calibration4S";
            // 
            // chk_Rotate90Deg
            // 
            resources.ApplyResources(this.chk_Rotate90Deg, "chk_Rotate90Deg");
            this.chk_Rotate90Deg.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Rotate90Deg.Name = "chk_Rotate90Deg";
            this.chk_Rotate90Deg.Selected = false;
            this.chk_Rotate90Deg.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Rotate90Deg.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Rotate90Deg.UseVisualStyleBackColor = true;
            // 
            // chk_UseGauge
            // 
            resources.ApplyResources(this.chk_UseGauge, "chk_UseGauge");
            this.chk_UseGauge.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_UseGauge.Name = "chk_UseGauge";
            this.chk_UseGauge.Selected = false;
            this.chk_UseGauge.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_UseGauge.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_UseGauge.UseVisualStyleBackColor = true;
            // 
            // txt_ZDistance
            // 
            resources.ApplyResources(this.txt_ZDistance, "txt_ZDistance");
            this.txt_ZDistance.BackColor = System.Drawing.Color.White;
            this.txt_ZDistance.DecimalPlaces = 3;
            this.txt_ZDistance.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ZDistance.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_ZDistance.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ZDistance.ForeColor = System.Drawing.Color.Black;
            this.txt_ZDistance.InputType = SRMControl.InputType.Number;
            this.txt_ZDistance.Name = "txt_ZDistance";
            this.txt_ZDistance.NormalBackColor = System.Drawing.Color.White;
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // tp_Offset
            // 
            resources.ApplyResources(this.tp_Offset, "tp_Offset");
            this.tp_Offset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Offset.Controls.Add(this.groupBox1);
            this.tp_Offset.Controls.Add(this.groupBox14);
            this.tp_Offset.Name = "tp_Offset";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.btn_Set);
            this.groupBox1.Controls.Add(this.label29);
            this.groupBox1.Controls.Add(this.txt_CalTol);
            this.groupBox1.Controls.Add(this.label30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btn_Set
            // 
            resources.ApplyResources(this.btn_Set, "btn_Set");
            this.btn_Set.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Set.Name = "btn_Set";
            this.btn_Set.UseVisualStyleBackColor = true;
            this.btn_Set.Click += new System.EventHandler(this.btn_Set_Click);
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            // 
            // txt_CalTol
            // 
            resources.ApplyResources(this.txt_CalTol, "txt_CalTol");
            this.txt_CalTol.BackColor = System.Drawing.Color.White;
            this.txt_CalTol.DecimalPlaces = 4;
            this.txt_CalTol.DecMaxValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_CalTol.DecMinValue = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.txt_CalTol.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_CalTol.ForeColor = System.Drawing.Color.Black;
            this.txt_CalTol.InputType = SRMControl.InputType.Number;
            this.txt_CalTol.Name = "txt_CalTol";
            this.txt_CalTol.NormalBackColor = System.Drawing.Color.White;
            this.txt_CalTol.TextChanged += new System.EventHandler(this.txt_CalTol_TextChanged);
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // groupBox14
            // 
            resources.ApplyResources(this.groupBox14, "groupBox14");
            this.groupBox14.Controls.Add(this.btn_ApplyCurrentValue);
            this.groupBox14.Controls.Add(this.label19);
            this.groupBox14.Controls.Add(this.txt_ZOffSet);
            this.groupBox14.Controls.Add(this.label20);
            this.groupBox14.Controls.Add(this.label12);
            this.groupBox14.Controls.Add(this.label11);
            this.groupBox14.Controls.Add(this.txt_HeightOffSet);
            this.groupBox14.Controls.Add(this.txt_WidthOffSet);
            this.groupBox14.Controls.Add(this.label76);
            this.groupBox14.Controls.Add(this.label77);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.TabStop = false;
            // 
            // btn_ApplyCurrentValue
            // 
            resources.ApplyResources(this.btn_ApplyCurrentValue, "btn_ApplyCurrentValue");
            this.btn_ApplyCurrentValue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ApplyCurrentValue.Name = "btn_ApplyCurrentValue";
            this.btn_ApplyCurrentValue.UseVisualStyleBackColor = true;
            this.btn_ApplyCurrentValue.Click += new System.EventHandler(this.btn_ApplyCurrentValue_Click);
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // txt_ZOffSet
            // 
            resources.ApplyResources(this.txt_ZOffSet, "txt_ZOffSet");
            this.txt_ZOffSet.BackColor = System.Drawing.Color.White;
            this.txt_ZOffSet.DecimalPlaces = 2;
            this.txt_ZOffSet.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ZOffSet.DecMinValue = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txt_ZOffSet.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ZOffSet.ForeColor = System.Drawing.Color.Black;
            this.txt_ZOffSet.InputType = SRMControl.InputType.Number;
            this.txt_ZOffSet.Name = "txt_ZOffSet";
            this.txt_ZOffSet.NormalBackColor = System.Drawing.Color.White;
            this.txt_ZOffSet.TextChanged += new System.EventHandler(this.txt_ResolutionOffSet_TextChanged);
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // txt_HeightOffSet
            // 
            resources.ApplyResources(this.txt_HeightOffSet, "txt_HeightOffSet");
            this.txt_HeightOffSet.BackColor = System.Drawing.Color.White;
            this.txt_HeightOffSet.DecimalPlaces = 2;
            this.txt_HeightOffSet.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_HeightOffSet.DecMinValue = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txt_HeightOffSet.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_HeightOffSet.ForeColor = System.Drawing.Color.Black;
            this.txt_HeightOffSet.InputType = SRMControl.InputType.Number;
            this.txt_HeightOffSet.Name = "txt_HeightOffSet";
            this.txt_HeightOffSet.NormalBackColor = System.Drawing.Color.White;
            this.txt_HeightOffSet.TextChanged += new System.EventHandler(this.txt_ResolutionOffSet_TextChanged);
            // 
            // txt_WidthOffSet
            // 
            resources.ApplyResources(this.txt_WidthOffSet, "txt_WidthOffSet");
            this.txt_WidthOffSet.BackColor = System.Drawing.Color.White;
            this.txt_WidthOffSet.DecimalPlaces = 2;
            this.txt_WidthOffSet.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_WidthOffSet.DecMinValue = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txt_WidthOffSet.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_WidthOffSet.ForeColor = System.Drawing.Color.Black;
            this.txt_WidthOffSet.InputType = SRMControl.InputType.Number;
            this.txt_WidthOffSet.Name = "txt_WidthOffSet";
            this.txt_WidthOffSet.NormalBackColor = System.Drawing.Color.White;
            this.txt_WidthOffSet.TextChanged += new System.EventHandler(this.txt_ResolutionOffSet_TextChanged);
            // 
            // label76
            // 
            resources.ApplyResources(this.label76, "label76");
            this.label76.Name = "label76";
            // 
            // label77
            // 
            resources.ApplyResources(this.label77, "label77");
            this.label77.Name = "label77";
            // 
            // btn_AdvancedSettings
            // 
            resources.ApplyResources(this.btn_AdvancedSettings, "btn_AdvancedSettings");
            this.btn_AdvancedSettings.Name = "btn_AdvancedSettings";
            this.btn_AdvancedSettings.UseVisualStyleBackColor = true;
            this.btn_AdvancedSettings.Click += new System.EventHandler(this.btn_AdvancedSettings_Click);
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Calibrate
            // 
            resources.ApplyResources(this.btn_Calibrate, "btn_Calibrate");
            this.btn_Calibrate.Name = "btn_Calibrate";
            this.btn_Calibrate.UseVisualStyleBackColor = true;
            this.btn_Calibrate.Click += new System.EventHandler(this.btn_Calibrate_Click);
            // 
            // gb_Result
            // 
            resources.ApplyResources(this.gb_Result, "gb_Result");
            this.gb_Result.Controls.Add(this.lbl_FOV);
            this.gb_Result.Controls.Add(this.label31);
            this.gb_Result.Controls.Add(this.txt_FOV);
            this.gb_Result.Controls.Add(this.label22);
            this.gb_Result.Controls.Add(this.label23);
            this.gb_Result.Controls.Add(this.txt_MeasuredZ);
            this.gb_Result.Controls.Add(this.label25);
            this.gb_Result.Controls.Add(this.label26);
            this.gb_Result.Controls.Add(this.label27);
            this.gb_Result.Controls.Add(this.label28);
            this.gb_Result.Controls.Add(this.txt_MeasuredY);
            this.gb_Result.Controls.Add(this.txt_MeasuredX);
            this.gb_Result.Controls.Add(this.radioBtn_mmperPixel);
            this.gb_Result.Controls.Add(this.radioBtn_pixelpermm);
            this.gb_Result.Controls.Add(this.label24);
            this.gb_Result.Controls.Add(this.label17);
            this.gb_Result.Controls.Add(this.label21);
            this.gb_Result.Controls.Add(this.txt_ZResolution);
            this.gb_Result.Controls.Add(this.label1);
            this.gb_Result.Controls.Add(this.label6);
            this.gb_Result.Controls.Add(this.label41);
            this.gb_Result.Controls.Add(this.label42);
            this.gb_Result.Controls.Add(this.txt_YResolution);
            this.gb_Result.Controls.Add(this.txt_XResolution);
            this.gb_Result.Name = "gb_Result";
            this.gb_Result.TabStop = false;
            // 
            // lbl_FOV
            // 
            resources.ApplyResources(this.lbl_FOV, "lbl_FOV");
            this.lbl_FOV.Name = "lbl_FOV";
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.Name = "label31";
            // 
            // txt_FOV
            // 
            resources.ApplyResources(this.txt_FOV, "txt_FOV");
            this.txt_FOV.Name = "txt_FOV";
            this.txt_FOV.ReadOnly = true;
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // txt_MeasuredZ
            // 
            resources.ApplyResources(this.txt_MeasuredZ, "txt_MeasuredZ");
            this.txt_MeasuredZ.Name = "txt_MeasuredZ";
            this.txt_MeasuredZ.ReadOnly = true;
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // txt_MeasuredY
            // 
            resources.ApplyResources(this.txt_MeasuredY, "txt_MeasuredY");
            this.txt_MeasuredY.Name = "txt_MeasuredY";
            this.txt_MeasuredY.ReadOnly = true;
            // 
            // txt_MeasuredX
            // 
            resources.ApplyResources(this.txt_MeasuredX, "txt_MeasuredX");
            this.txt_MeasuredX.Name = "txt_MeasuredX";
            this.txt_MeasuredX.ReadOnly = true;
            // 
            // radioBtn_mmperPixel
            // 
            resources.ApplyResources(this.radioBtn_mmperPixel, "radioBtn_mmperPixel");
            this.radioBtn_mmperPixel.Name = "radioBtn_mmperPixel";
            this.radioBtn_mmperPixel.UseVisualStyleBackColor = true;
            this.radioBtn_mmperPixel.Click += new System.EventHandler(this.radioBtn_mmperPixel_Click);
            // 
            // radioBtn_pixelpermm
            // 
            resources.ApplyResources(this.radioBtn_pixelpermm, "radioBtn_pixelpermm");
            this.radioBtn_pixelpermm.Checked = true;
            this.radioBtn_pixelpermm.Name = "radioBtn_pixelpermm";
            this.radioBtn_pixelpermm.TabStop = true;
            this.radioBtn_pixelpermm.UseVisualStyleBackColor = true;
            this.radioBtn_pixelpermm.Click += new System.EventHandler(this.radioBtn_mmperPixel_Click);
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // txt_ZResolution
            // 
            resources.ApplyResources(this.txt_ZResolution, "txt_ZResolution");
            this.txt_ZResolution.Name = "txt_ZResolution";
            this.txt_ZResolution.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.Name = "label41";
            // 
            // label42
            // 
            resources.ApplyResources(this.label42, "label42");
            this.label42.Name = "label42";
            // 
            // txt_YResolution
            // 
            resources.ApplyResources(this.txt_YResolution, "txt_YResolution");
            this.txt_YResolution.Name = "txt_YResolution";
            this.txt_YResolution.ReadOnly = true;
            // 
            // txt_XResolution
            // 
            resources.ApplyResources(this.txt_XResolution, "txt_XResolution");
            this.txt_XResolution.Name = "txt_XResolution";
            this.txt_XResolution.ReadOnly = true;
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // txt_PixelSizeY
            // 
            resources.ApplyResources(this.txt_PixelSizeY, "txt_PixelSizeY");
            this.txt_PixelSizeY.Name = "txt_PixelSizeY";
            this.txt_PixelSizeY.ReadOnly = true;
            // 
            // txt_PixelSizeX
            // 
            resources.ApplyResources(this.txt_PixelSizeX, "txt_PixelSizeX");
            this.txt_PixelSizeX.Name = "txt_PixelSizeX";
            this.txt_PixelSizeX.ReadOnly = true;
            // 
            // lbl_ResultStatus
            // 
            resources.ApplyResources(this.lbl_ResultStatus, "lbl_ResultStatus");
            this.lbl_ResultStatus.BackColor = System.Drawing.Color.Gray;
            this.lbl_ResultStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ResultStatus.Name = "lbl_ResultStatus";
            // 
            // btn_Camera
            // 
            resources.ApplyResources(this.btn_Camera, "btn_Camera");
            this.btn_Camera.Name = "btn_Camera";
            this.btn_Camera.UseVisualStyleBackColor = true;
            this.btn_Camera.Click += new System.EventHandler(this.btn_Camera_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // CalibrationForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.btn_Camera);
            this.Controls.Add(this.lbl_ResultStatus);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.srmTabControl1);
            this.Controls.Add(this.btn_Calibrate);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_AdvancedSettings);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.gb_Result);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.txt_PixelSizeX);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txt_PixelSizeY);
            this.Controls.Add(this.label16);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CalibrationForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalibrationForm_FormClosing);
            this.Load += new System.EventHandler(this.CalibrationForm_Load);
            this.srmTabControl1.ResumeLayout(false);
            this.tabPage_Calibration.ResumeLayout(false);
            this.tabPage_Calibration.PerformLayout();
            this.tabPage_CalibrationByCircle.ResumeLayout(false);
            this.tabPage_CalibrationByCircle.PerformLayout();
            this.tabPage_CalibrationByXY.ResumeLayout(false);
            this.tabPage_CalibrationByXY.PerformLayout();
            this.tabPage_Calibration4S.ResumeLayout(false);
            this.tabPage_Calibration4S.PerformLayout();
            this.tp_Offset.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.gb_Result.ResumeLayout(false);
            this.gb_Result.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMTabControl srmTabControl1;
        private System.Windows.Forms.TabPage tabPage_Calibration;
        private SRMControl.SRMButton btn_Threshold;
        private SRMControl.SRMInputBox txt_MaxArea;
        private SRMControl.SRMInputBox txt_MinArea;
        private SRMControl.SRMInputBox txt_GridPitchY;
        private SRMControl.SRMInputBox txt_GridPitchX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private SRMControl.SRMButton btn_AdvancedSettings;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_Calibrate;
        private System.Windows.Forms.GroupBox gb_Result;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.TextBox txt_XResolution;
        private System.Windows.Forms.GroupBox groupBox14;
        private SRMControl.SRMInputBox txt_HeightOffSet;
        private SRMControl.SRMInputBox txt_WidthOffSet;
        private System.Windows.Forms.Label label76;
        private System.Windows.Forms.Label label77;
        private System.Windows.Forms.TabPage tabPage_CalibrationByCircle;
        private System.Windows.Forms.ComboBox cbo_TransitionType;
        private System.Windows.Forms.ComboBox cbo_TransitionChoice;
        private System.Windows.Forms.Label label79;
        private System.Windows.Forms.Label label78;
        private SRMControl.SRMInputBox txt_Diameter;
        private System.Windows.Forms.Label label56;
        private SRMControl.SRMInputBox txt_GaugeThreshold;
        private System.Windows.Forms.Label label8;
        private SRMControl.SRMInputBox txt_GaugeThickness;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage_CalibrationByXY;
        private SRMControl.SRMInputBox txt_YDistance;
        private System.Windows.Forms.Label label9;
        private SRMControl.SRMInputBox txt_XDistance;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txt_PixelSizeY;
        private System.Windows.Forms.TextBox txt_PixelSizeX;
        private System.Windows.Forms.TabPage tabPage_Calibration4S;
        private SRMControl.SRMInputBox txt_ZDistance;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private SRMControl.SRMInputBox txt_ZOffSet;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txt_ZResolution;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.TextBox txt_YResolution;
        private SRMControl.SRMRadioButton radioBtn_mmperPixel;
        private SRMControl.SRMRadioButton radioBtn_pixelpermm;
        private System.Windows.Forms.Label label24;
        private SRMControl.SRMCheckBox chk_Rotate90Deg;
        private SRMControl.SRMCheckBox chk_UseGauge;
        private System.Windows.Forms.Label lbl_ResultStatus;
        private SRMControl.SRMButton btn_Camera;
        private SRMControl.SRMButton btn_GaugeAdvanceSetting;
        private SRMControl.SRMCheckBox chk_UseGauge1;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txt_MeasuredZ;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox txt_MeasuredY;
        private System.Windows.Forms.TextBox txt_MeasuredX;
        private System.Windows.Forms.GroupBox groupBox1;
        private SRMControl.SRMButton btn_Set;
        private System.Windows.Forms.Label label29;
        private SRMControl.SRMInputBox txt_CalTol;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TabPage tp_Offset;
        private SRMControl.SRMCheckBox chk_ShowDraggingBox;
        private SRMControl.SRMCheckBox chk_ShowSamplePoints;
        private System.Windows.Forms.Timer timer1;
        private SRMControl.SRMButton btn_ApplyCurrentValue;
        private System.Windows.Forms.Label lbl_FOV;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox txt_FOV;
    }
}