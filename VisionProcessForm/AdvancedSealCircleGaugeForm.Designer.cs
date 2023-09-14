namespace VisionProcessForm
{
    partial class AdvancedSealCircleGaugeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedSealCircleGaugeForm));
            this.txt_Tolerance = new SRMControl.SRMInputBox();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.btn_OK = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.txt_FilteringThreshold = new SRMControl.SRMInputBox();
            this.lbl_SamplingStep = new SRMControl.SRMLabel();
            this.txt_SamplingStep = new SRMControl.SRMInputBox();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.txt_MeasMinArea = new SRMControl.SRMInputBox();
            this.txt_FilteringPass = new SRMControl.SRMInputBox();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_MeasFilter = new SRMControl.SRMInputBox();
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
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.txt_MeasThickness = new SRMControl.SRMInputBox();
            this.trackBar_Thickness = new System.Windows.Forms.TrackBar();
            this.trackBar_Derivative = new System.Windows.Forms.TrackBar();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.txt_GaugeScore = new SRMControl.SRMInputBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_MinAmp)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Thickness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Derivative)).BeginInit();
            this.SuspendLayout();
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
            this.txt_Tolerance.TextChanged += new System.EventHandler(this.txt_Tolerance_TextChanged);
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
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
            // lbl_SamplingStep
            // 
            resources.ApplyResources(this.lbl_SamplingStep, "lbl_SamplingStep");
            this.lbl_SamplingStep.Name = "lbl_SamplingStep";
            this.lbl_SamplingStep.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_SamplingStep
            // 
            resources.ApplyResources(this.txt_SamplingStep, "txt_SamplingStep");
            this.txt_SamplingStep.BackColor = System.Drawing.Color.White;
            this.txt_SamplingStep.DataType = SRMControl.SRMDataType.Int32;
            this.txt_SamplingStep.DecimalPlaces = 0;
            this.txt_SamplingStep.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_SamplingStep.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_SamplingStep.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_SamplingStep.ForeColor = System.Drawing.Color.Black;
            this.txt_SamplingStep.InputType = SRMControl.InputType.Number;
            this.txt_SamplingStep.Name = "txt_SamplingStep";
            this.txt_SamplingStep.NormalBackColor = System.Drawing.Color.White;
            this.txt_SamplingStep.TextChanged += new System.EventHandler(this.txt_SamplingStep_TextChanged);
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
            // srmLabel6
            // 
            resources.ApplyResources(this.srmLabel6, "srmLabel6");
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_GaugeScore
            // 
            resources.ApplyResources(this.txt_GaugeScore, "txt_GaugeScore");
            this.txt_GaugeScore.BackColor = System.Drawing.Color.White;
            this.txt_GaugeScore.DataType = SRMControl.SRMDataType.Int32;
            this.txt_GaugeScore.DecimalPlaces = 0;
            this.txt_GaugeScore.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_GaugeScore.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_GaugeScore.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_GaugeScore.ForeColor = System.Drawing.Color.Black;
            this.txt_GaugeScore.InputType = SRMControl.InputType.Number;
            this.txt_GaugeScore.Name = "txt_GaugeScore";
            this.txt_GaugeScore.NormalBackColor = System.Drawing.Color.White;
            this.txt_GaugeScore.TextChanged += new System.EventHandler(this.txt_GaugeScore_TextChanged);
            // 
            // AdvancedSealCircleGaugeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.srmLabel6);
            this.Controls.Add(this.txt_GaugeScore);
            this.Controls.Add(this.txt_Tolerance);
            this.Controls.Add(this.srmLabel5);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.txt_FilteringThreshold);
            this.Controls.Add(this.lbl_SamplingStep);
            this.Controls.Add(this.txt_SamplingStep);
            this.Controls.Add(this.srmLabel11);
            this.Controls.Add(this.srmLabel4);
            this.Controls.Add(this.txt_MeasMinArea);
            this.Controls.Add(this.txt_FilteringPass);
            this.Controls.Add(this.srmLabel13);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.txt_MeasFilter);
            this.Controls.Add(this.trackBar_MinAmp);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.srmLabel10);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.txt_MeasMinAmp);
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.txt_threshold);
            this.Controls.Add(this.srmLabel12);
            this.Controls.Add(this.srmLabel8);
            this.Controls.Add(this.txt_MeasThickness);
            this.Controls.Add(this.trackBar_Thickness);
            this.Controls.Add(this.trackBar_Derivative);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancedSealCircleGaugeForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdvancedSealCircleGaugeForm_FormClosing);
            this.Load += new System.EventHandler(this.AdvancedSealCircleGaugeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_MinAmp)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Thickness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Derivative)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMInputBox txt_Tolerance;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMInputBox txt_FilteringThreshold;
        private SRMControl.SRMLabel lbl_SamplingStep;
        private SRMControl.SRMInputBox txt_SamplingStep;
        private SRMControl.SRMLabel srmLabel11;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMInputBox txt_MeasMinArea;
        private SRMControl.SRMInputBox txt_FilteringPass;
        private SRMControl.SRMLabel srmLabel13;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox txt_MeasFilter;
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
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMInputBox txt_MeasThickness;
        private System.Windows.Forms.TrackBar trackBar_Thickness;
        private System.Windows.Forms.TrackBar trackBar_Derivative;
        private SRMControl.SRMLabel srmLabel6;
        private SRMControl.SRMInputBox txt_GaugeScore;
    }
}