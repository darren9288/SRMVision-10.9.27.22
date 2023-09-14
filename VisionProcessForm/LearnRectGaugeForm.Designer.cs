namespace VisionProcessForm
{
    partial class LearnRectGaugeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearnRectGaugeForm));
            this.lbl_Title = new System.Windows.Forms.Label();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.tbCtrl_Gauge = new SRMControl.SRMTabControl();
            this.tp_Position = new System.Windows.Forms.TabPage();
            this.txt_Size = new SRMControl.SRMInputBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_PosTolerance = new SRMControl.SRMInputBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.group_TransSelect = new SRMControl.SRMGroupBox();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.cbo_TransChoice = new SRMControl.SRMComboBox();
            this.cbo_TransType = new SRMControl.SRMImageComboBox();
            this.tp_Measurement = new System.Windows.Forms.TabPage();
            this.txt_FilteringThreshold = new SRMControl.SRMInputBox();
            this.txt_MeasFilter = new SRMControl.SRMInputBox();
            this.txt_MeasMinArea = new SRMControl.SRMInputBox();
            this.txt_MeasMinAmp = new SRMControl.SRMInputBox();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.txt_FilteringPass = new SRMControl.SRMInputBox();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.tp_Fitting = new System.Windows.Forms.TabPage();
            this.txt_MeasThreshold = new SRMControl.SRMInputBox();
            this.txt_MeasThickness = new SRMControl.SRMInputBox();
            this.srmLabel9 = new SRMControl.SRMLabel();
            this.txt_FittingSamplingStep = new SRMControl.SRMInputBox();
            this.srmLabel14 = new SRMControl.SRMLabel();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.lbl_AdvancedSettings = new SRMControl.SRMLabel();
            this.gb_GainSetting = new SRMControl.SRMGroupBox();
            this.txt_GainValue = new SRMControl.SRMInputBox();
            this.trackBar_Gain = new System.Windows.Forms.TrackBar();
            this.chk_UseMarkOrientGauge = new SRMControl.SRMCheckBox();
            this.cbo_ImagesList = new SRMControl.SRMImageComboBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.cbo_RectGList = new SRMControl.SRMImageComboBox();
            this.chk_WantGauge = new SRMControl.SRMCheckBox();
            this.tbCtrl_Gauge.SuspendLayout();
            this.tp_Position.SuspendLayout();
            this.group_TransSelect.SuspendLayout();
            this.tp_Measurement.SuspendLayout();
            this.tp_Fitting.SuspendLayout();
            this.gb_GainSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Gain)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Title
            // 
            resources.ApplyResources(this.lbl_Title, "lbl_Title");
            this.lbl_Title.ForeColor = System.Drawing.Color.Blue;
            this.lbl_Title.Name = "lbl_Title";
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tbCtrl_Gauge
            // 
            this.tbCtrl_Gauge.Controls.Add(this.tp_Position);
            this.tbCtrl_Gauge.Controls.Add(this.tp_Measurement);
            this.tbCtrl_Gauge.Controls.Add(this.tp_Fitting);
            resources.ApplyResources(this.tbCtrl_Gauge, "tbCtrl_Gauge");
            this.tbCtrl_Gauge.Name = "tbCtrl_Gauge";
            this.tbCtrl_Gauge.SelectedIndex = 0;
            // 
            // tp_Position
            // 
            this.tp_Position.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Position.Controls.Add(this.txt_Size);
            this.tp_Position.Controls.Add(this.label4);
            this.tp_Position.Controls.Add(this.txt_PosTolerance);
            this.tp_Position.Controls.Add(this.label3);
            this.tp_Position.Controls.Add(this.label2);
            this.tp_Position.Controls.Add(this.group_TransSelect);
            resources.ApplyResources(this.tp_Position, "tp_Position");
            this.tp_Position.Name = "tp_Position";
            // 
            // txt_Size
            // 
            this.txt_Size.BackColor = System.Drawing.Color.White;
            this.txt_Size.DecimalPlaces = 0;
            this.txt_Size.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_Size.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Size.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Size.ForeColor = System.Drawing.Color.Black;
            this.txt_Size.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_Size, "txt_Size");
            this.txt_Size.Name = "txt_Size";
            this.txt_Size.NormalBackColor = System.Drawing.Color.White;
            this.txt_Size.TextChanged += new System.EventHandler(this.txt_Size_TextChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txt_PosTolerance
            // 
            this.txt_PosTolerance.BackColor = System.Drawing.Color.White;
            this.txt_PosTolerance.DataType = SRMControl.SRMDataType.Int32;
            this.txt_PosTolerance.DecimalPlaces = 0;
            this.txt_PosTolerance.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_PosTolerance.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_PosTolerance.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_PosTolerance.ForeColor = System.Drawing.Color.Black;
            this.txt_PosTolerance.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_PosTolerance, "txt_PosTolerance");
            this.txt_PosTolerance.Name = "txt_PosTolerance";
            this.txt_PosTolerance.NormalBackColor = System.Drawing.Color.White;
            this.txt_PosTolerance.TextChanged += new System.EventHandler(this.txt_PosTolerance_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // group_TransSelect
            // 
            this.group_TransSelect.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_TransSelect.Controls.Add(this.srmLabel8);
            this.group_TransSelect.Controls.Add(this.srmLabel7);
            this.group_TransSelect.Controls.Add(this.cbo_TransChoice);
            this.group_TransSelect.Controls.Add(this.cbo_TransType);
            resources.ApplyResources(this.group_TransSelect, "group_TransSelect");
            this.group_TransSelect.Name = "group_TransSelect";
            this.group_TransSelect.TabStop = false;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_TransChoice
            // 
            this.cbo_TransChoice.BackColor = System.Drawing.Color.White;
            this.cbo_TransChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_TransChoice.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_TransChoice.FormattingEnabled = true;
            this.cbo_TransChoice.Items.AddRange(new object[] {
            resources.GetString("cbo_TransChoice.Items"),
            resources.GetString("cbo_TransChoice.Items1"),
            resources.GetString("cbo_TransChoice.Items2"),
            resources.GetString("cbo_TransChoice.Items3"),
            resources.GetString("cbo_TransChoice.Items4")});
            resources.ApplyResources(this.cbo_TransChoice, "cbo_TransChoice");
            this.cbo_TransChoice.Name = "cbo_TransChoice";
            this.cbo_TransChoice.NormalBackColor = System.Drawing.Color.White;
            this.cbo_TransChoice.SelectedIndexChanged += new System.EventHandler(this.cbo_TransChoice_SelectedIndexChanged);
            // 
            // cbo_TransType
            // 
            this.cbo_TransType.BackColor = System.Drawing.Color.White;
            this.cbo_TransType.DisplayMember = "ItemData";
            this.cbo_TransType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_TransType.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_TransType.FormattingEnabled = true;
            this.cbo_TransType.Items.AddRange(new object[] {
            resources.GetString("cbo_TransType.Items"),
            resources.GetString("cbo_TransType.Items1"),
            resources.GetString("cbo_TransType.Items2"),
            resources.GetString("cbo_TransType.Items3"),
            resources.GetString("cbo_TransType.Items4")});
            resources.ApplyResources(this.cbo_TransType, "cbo_TransType");
            this.cbo_TransType.Name = "cbo_TransType";
            this.cbo_TransType.NormalBackColor = System.Drawing.Color.White;
            this.cbo_TransType.SelectedIndexChanged += new System.EventHandler(this.cbo_TransType_SelectedIndexChanged);
            // 
            // tp_Measurement
            // 
            this.tp_Measurement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Measurement.Controls.Add(this.txt_FilteringThreshold);
            this.tp_Measurement.Controls.Add(this.txt_MeasFilter);
            this.tp_Measurement.Controls.Add(this.txt_MeasMinArea);
            this.tp_Measurement.Controls.Add(this.txt_MeasMinAmp);
            this.tp_Measurement.Controls.Add(this.srmLabel13);
            this.tp_Measurement.Controls.Add(this.txt_FilteringPass);
            this.tp_Measurement.Controls.Add(this.srmLabel11);
            this.tp_Measurement.Controls.Add(this.srmLabel10);
            this.tp_Measurement.Controls.Add(this.srmLabel2);
            this.tp_Measurement.Controls.Add(this.srmLabel1);
            resources.ApplyResources(this.tp_Measurement, "tp_Measurement");
            this.tp_Measurement.Name = "tp_Measurement";
            // 
            // txt_FilteringThreshold
            // 
            this.txt_FilteringThreshold.BackColor = System.Drawing.Color.White;
            this.txt_FilteringThreshold.DataType = SRMControl.SRMDataType.Int32;
            this.txt_FilteringThreshold.DecimalPlaces = 1;
            this.txt_FilteringThreshold.DecMaxValue = new decimal(new int[] {
            50,
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
            resources.ApplyResources(this.txt_FilteringThreshold, "txt_FilteringThreshold");
            this.txt_FilteringThreshold.Name = "txt_FilteringThreshold";
            this.txt_FilteringThreshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_FilteringThreshold.TextChanged += new System.EventHandler(this.txt_FilteringThreshold_TextChanged);
            // 
            // txt_MeasFilter
            // 
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
            resources.ApplyResources(this.txt_MeasFilter, "txt_MeasFilter");
            this.txt_MeasFilter.Name = "txt_MeasFilter";
            this.txt_MeasFilter.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasFilter.TextChanged += new System.EventHandler(this.txt_MeasFilter_TextChanged);
            // 
            // txt_MeasMinArea
            // 
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
            resources.ApplyResources(this.txt_MeasMinArea, "txt_MeasMinArea");
            this.txt_MeasMinArea.Name = "txt_MeasMinArea";
            this.txt_MeasMinArea.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasMinArea.TextChanged += new System.EventHandler(this.txt_MeasMinArea_TextChanged);
            // 
            // txt_MeasMinAmp
            // 
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
            resources.ApplyResources(this.txt_MeasMinAmp, "txt_MeasMinAmp");
            this.txt_MeasMinAmp.Name = "txt_MeasMinAmp";
            this.txt_MeasMinAmp.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasMinAmp.TextChanged += new System.EventHandler(this.txt_MeasMinAmp_TextChanged);
            // 
            // srmLabel13
            // 
            resources.ApplyResources(this.srmLabel13, "srmLabel13");
            this.srmLabel13.Name = "srmLabel13";
            this.srmLabel13.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_FilteringPass
            // 
            this.txt_FilteringPass.BackColor = System.Drawing.Color.White;
            this.txt_FilteringPass.DataType = SRMControl.SRMDataType.Int32;
            this.txt_FilteringPass.DecimalPlaces = 0;
            this.txt_FilteringPass.DecMaxValue = new decimal(new int[] {
            50,
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
            resources.ApplyResources(this.txt_FilteringPass, "txt_FilteringPass");
            this.txt_FilteringPass.Name = "txt_FilteringPass";
            this.txt_FilteringPass.NormalBackColor = System.Drawing.Color.White;
            this.txt_FilteringPass.TextChanged += new System.EventHandler(this.txt_FilteringPass_TextChanged);
            // 
            // srmLabel11
            // 
            resources.ApplyResources(this.srmLabel11, "srmLabel11");
            this.srmLabel11.Name = "srmLabel11";
            this.srmLabel11.TextShadowColor = System.Drawing.Color.Gray;
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
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tp_Fitting
            // 
            this.tp_Fitting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Fitting.Controls.Add(this.txt_MeasThreshold);
            this.tp_Fitting.Controls.Add(this.txt_MeasThickness);
            this.tp_Fitting.Controls.Add(this.srmLabel9);
            this.tp_Fitting.Controls.Add(this.txt_FittingSamplingStep);
            this.tp_Fitting.Controls.Add(this.srmLabel14);
            this.tp_Fitting.Controls.Add(this.srmLabel12);
            resources.ApplyResources(this.tp_Fitting, "tp_Fitting");
            this.tp_Fitting.Name = "tp_Fitting";
            // 
            // txt_MeasThreshold
            // 
            this.txt_MeasThreshold.BackColor = System.Drawing.Color.White;
            this.txt_MeasThreshold.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MeasThreshold.DecimalPlaces = 0;
            this.txt_MeasThreshold.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_MeasThreshold.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MeasThreshold.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MeasThreshold.ForeColor = System.Drawing.Color.Black;
            this.txt_MeasThreshold.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_MeasThreshold, "txt_MeasThreshold");
            this.txt_MeasThreshold.Name = "txt_MeasThreshold";
            this.txt_MeasThreshold.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasThreshold.TextChanged += new System.EventHandler(this.txt_MeasThreshold_TextChanged);
            // 
            // txt_MeasThickness
            // 
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
            resources.ApplyResources(this.txt_MeasThickness, "txt_MeasThickness");
            this.txt_MeasThickness.Name = "txt_MeasThickness";
            this.txt_MeasThickness.NormalBackColor = System.Drawing.Color.White;
            this.txt_MeasThickness.TextChanged += new System.EventHandler(this.txt_MeasThickness_TextChanged);
            // 
            // srmLabel9
            // 
            resources.ApplyResources(this.srmLabel9, "srmLabel9");
            this.srmLabel9.Name = "srmLabel9";
            this.srmLabel9.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_FittingSamplingStep
            // 
            this.txt_FittingSamplingStep.BackColor = System.Drawing.Color.White;
            this.txt_FittingSamplingStep.DataType = SRMControl.SRMDataType.Int32;
            this.txt_FittingSamplingStep.DecimalPlaces = 0;
            this.txt_FittingSamplingStep.DecMaxValue = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.txt_FittingSamplingStep.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_FittingSamplingStep.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_FittingSamplingStep.ForeColor = System.Drawing.Color.Black;
            this.txt_FittingSamplingStep.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_FittingSamplingStep, "txt_FittingSamplingStep");
            this.txt_FittingSamplingStep.Name = "txt_FittingSamplingStep";
            this.txt_FittingSamplingStep.NormalBackColor = System.Drawing.Color.White;
            this.txt_FittingSamplingStep.TextChanged += new System.EventHandler(this.txt_FittingSamplingStep_TextChanged);
            // 
            // srmLabel14
            // 
            resources.ApplyResources(this.srmLabel14, "srmLabel14");
            this.srmLabel14.Name = "srmLabel14";
            this.srmLabel14.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel12
            // 
            resources.ApplyResources(this.srmLabel12, "srmLabel12");
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_AdvancedSettings
            // 
            resources.ApplyResources(this.lbl_AdvancedSettings, "lbl_AdvancedSettings");
            this.lbl_AdvancedSettings.Name = "lbl_AdvancedSettings";
            this.lbl_AdvancedSettings.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // gb_GainSetting
            // 
            this.gb_GainSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.gb_GainSetting.Controls.Add(this.txt_GainValue);
            this.gb_GainSetting.Controls.Add(this.trackBar_Gain);
            resources.ApplyResources(this.gb_GainSetting, "gb_GainSetting");
            this.gb_GainSetting.Name = "gb_GainSetting";
            this.gb_GainSetting.TabStop = false;
            // 
            // txt_GainValue
            // 
            this.txt_GainValue.BackColor = System.Drawing.Color.White;
            this.txt_GainValue.DecimalPlaces = 3;
            this.txt_GainValue.DecMaxValue = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txt_GainValue.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_GainValue.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_GainValue.ForeColor = System.Drawing.Color.Black;
            this.txt_GainValue.InputType = SRMControl.InputType.Number;
            resources.ApplyResources(this.txt_GainValue, "txt_GainValue");
            this.txt_GainValue.Name = "txt_GainValue";
            this.txt_GainValue.NormalBackColor = System.Drawing.Color.White;
            this.txt_GainValue.TextChanged += new System.EventHandler(this.txt_GainValue_TextChanged);
            // 
            // trackBar_Gain
            // 
            resources.ApplyResources(this.trackBar_Gain, "trackBar_Gain");
            this.trackBar_Gain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Gain.LargeChange = 1;
            this.trackBar_Gain.Maximum = 10000;
            this.trackBar_Gain.Name = "trackBar_Gain";
            this.trackBar_Gain.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Gain.Value = 1000;
            this.trackBar_Gain.Scroll += new System.EventHandler(this.trackBar_Gain_Scroll);
            // 
            // chk_UseMarkOrientGauge
            // 
            this.chk_UseMarkOrientGauge.CheckedColor = System.Drawing.Color.GreenYellow;
            resources.ApplyResources(this.chk_UseMarkOrientGauge, "chk_UseMarkOrientGauge");
            this.chk_UseMarkOrientGauge.Name = "chk_UseMarkOrientGauge";
            this.chk_UseMarkOrientGauge.Selected = false;
            this.chk_UseMarkOrientGauge.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_UseMarkOrientGauge.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_UseMarkOrientGauge.UseVisualStyleBackColor = true;
            this.chk_UseMarkOrientGauge.Click += new System.EventHandler(this.chk_UseMarkOrientGauge_Click);
            // 
            // cbo_ImagesList
            // 
            this.cbo_ImagesList.BackColor = System.Drawing.Color.White;
            this.cbo_ImagesList.DisplayMember = "ItemData";
            this.cbo_ImagesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ImagesList.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ImagesList.FormattingEnabled = true;
            resources.ApplyResources(this.cbo_ImagesList, "cbo_ImagesList");
            this.cbo_ImagesList.Name = "cbo_ImagesList";
            this.cbo_ImagesList.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ImagesList.SelectedIndexChanged += new System.EventHandler(this.cbo_ImagesList_SelectedIndexChanged);
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_RectGList
            // 
            this.cbo_RectGList.BackColor = System.Drawing.Color.White;
            this.cbo_RectGList.DisplayMember = "ItemData";
            this.cbo_RectGList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_RectGList.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_RectGList.FormattingEnabled = true;
            resources.ApplyResources(this.cbo_RectGList, "cbo_RectGList");
            this.cbo_RectGList.Name = "cbo_RectGList";
            this.cbo_RectGList.NormalBackColor = System.Drawing.Color.White;
            this.cbo_RectGList.SelectedIndexChanged += new System.EventHandler(this.cbo_RectGList_SelectedIndexChanged);
            // 
            // chk_WantGauge
            // 
            resources.ApplyResources(this.chk_WantGauge, "chk_WantGauge");
            this.chk_WantGauge.CheckedColor = System.Drawing.Color.LawnGreen;
            this.chk_WantGauge.Name = "chk_WantGauge";
            this.chk_WantGauge.Selected = false;
            this.chk_WantGauge.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantGauge.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_WantGauge.CheckedChanged += new System.EventHandler(this.chk_WantGauge_CheckedChanged);
            this.chk_WantGauge.Click += new System.EventHandler(this.chk_WantGauge_Click);
            // 
            // LearnRectGaugeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.chk_WantGauge);
            this.Controls.Add(this.cbo_ImagesList);
            this.Controls.Add(this.cbo_RectGList);
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.chk_UseMarkOrientGauge);
            this.Controls.Add(this.gb_GainSetting);
            this.Controls.Add(this.lbl_AdvancedSettings);
            this.Controls.Add(this.tbCtrl_Gauge);
            this.Controls.Add(this.srmLabel4);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.lbl_Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LearnRectGaugeForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LearnRectGaugeForm_FormClosing);
            this.Load += new System.EventHandler(this.LearnRectGaugeForm_Load);
            this.tbCtrl_Gauge.ResumeLayout(false);
            this.tp_Position.ResumeLayout(false);
            this.tp_Position.PerformLayout();
            this.group_TransSelect.ResumeLayout(false);
            this.group_TransSelect.PerformLayout();
            this.tp_Measurement.ResumeLayout(false);
            this.tp_Measurement.PerformLayout();
            this.tp_Fitting.ResumeLayout(false);
            this.tp_Fitting.PerformLayout();
            this.gb_GainSetting.ResumeLayout(false);
            this.gb_GainSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Gain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_Title;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMTabControl tbCtrl_Gauge;
        private System.Windows.Forms.TabPage tp_Position;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tp_Measurement;
        private SRMControl.SRMLabel srmLabel13;
        private SRMControl.SRMLabel srmLabel12;
        private SRMControl.SRMLabel srmLabel11;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMLabel srmLabel9;
        private SRMControl.SRMGroupBox group_TransSelect;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMComboBox cbo_TransChoice;
        private SRMControl.SRMImageComboBox cbo_TransType;
        private System.Windows.Forms.TabPage tp_Fitting;
        private SRMControl.SRMLabel srmLabel14;
        private SRMControl.SRMLabel lbl_AdvancedSettings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private SRMControl.SRMInputBox txt_Size;
        private SRMControl.SRMInputBox txt_PosTolerance;
        private SRMControl.SRMInputBox txt_MeasFilter;
        private SRMControl.SRMInputBox txt_MeasThickness;
        private SRMControl.SRMInputBox txt_MeasMinArea;
        private SRMControl.SRMInputBox txt_MeasMinAmp;
        private SRMControl.SRMInputBox txt_MeasThreshold;
        private SRMControl.SRMInputBox txt_FittingSamplingStep;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox txt_FilteringThreshold;
        private SRMControl.SRMInputBox txt_FilteringPass;
        private SRMControl.SRMGroupBox gb_GainSetting;
        private System.Windows.Forms.TrackBar trackBar_Gain;
        private SRMControl.SRMCheckBox chk_UseMarkOrientGauge;
        private SRMControl.SRMImageComboBox cbo_ImagesList;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMImageComboBox cbo_RectGList;
        private SRMControl.SRMInputBox txt_GainValue;
        private SRMControl.SRMCheckBox chk_WantGauge;
    }
}