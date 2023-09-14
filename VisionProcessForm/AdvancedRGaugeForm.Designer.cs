namespace VisionProcessForm
{
    partial class AdvancedRGaugeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedRGaugeForm));
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.cbo_TransChoice = new SRMControl.SRMComboBox();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.srmLabel12 = new SRMControl.SRMLabel();
            this.srmLabel11 = new SRMControl.SRMLabel();
            this.srmLabel10 = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_threshold = new System.Windows.Forms.NumericUpDown();
            this.txt_MinAmplitude = new System.Windows.Forms.NumericUpDown();
            this.txt_MinArea = new System.Windows.Forms.NumericUpDown();
            this.txt_Thickness = new System.Windows.Forms.NumericUpDown();
            this.txt_Filter = new System.Windows.Forms.NumericUpDown();
            this.txt_SizeTolerance = new System.Windows.Forms.NumericUpDown();
            this.srmlabel100 = new SRMControl.SRMLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.cbo_TransType = new SRMControl.SRMComboBox();
            this.txt_Tolerance = new System.Windows.Forms.NumericUpDown();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.txt_FilteringPass = new System.Windows.Forms.NumericUpDown();
            this.txt_FilteringThreshold = new System.Windows.Forms.NumericUpDown();
            this.gb_GainSetting = new SRMControl.SRMGroupBox();
            this.txt_GainValue = new SRMControl.SRMInputBox();
            this.trackBar_Gain = new System.Windows.Forms.TrackBar();
            this.txt_FittingSamplingStep = new System.Windows.Forms.NumericUpDown();
            this.srmLabel6 = new SRMControl.SRMLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txt_threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_MinAmplitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_MinArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Thickness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Filter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_SizeTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Tolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_FilteringPass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_FilteringThreshold)).BeginInit();
            this.gb_GainSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Gain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_FittingSamplingStep)).BeginInit();
            this.SuspendLayout();
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
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
            // srmLabel13
            // 
            resources.ApplyResources(this.srmLabel13, "srmLabel13");
            this.srmLabel13.Name = "srmLabel13";
            this.srmLabel13.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel12
            // 
            resources.ApplyResources(this.srmLabel12, "srmLabel12");
            this.srmLabel12.Name = "srmLabel12";
            this.srmLabel12.TextShadowColor = System.Drawing.Color.Gray;
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
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_threshold
            // 
            resources.ApplyResources(this.txt_threshold, "txt_threshold");
            this.txt_threshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_threshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_threshold.Name = "txt_threshold";
            this.txt_threshold.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.txt_threshold.ValueChanged += new System.EventHandler(this.txt_threshold_ValueChanged);
            // 
            // txt_MinAmplitude
            // 
            resources.ApplyResources(this.txt_MinAmplitude, "txt_MinAmplitude");
            this.txt_MinAmplitude.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_MinAmplitude.Name = "txt_MinAmplitude";
            this.txt_MinAmplitude.ValueChanged += new System.EventHandler(this.txt_MinAmplitude_ValueChanged);
            // 
            // txt_MinArea
            // 
            resources.ApplyResources(this.txt_MinArea, "txt_MinArea");
            this.txt_MinArea.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.txt_MinArea.Name = "txt_MinArea";
            this.txt_MinArea.ValueChanged += new System.EventHandler(this.txt_MinArea_ValueChanged);
            // 
            // txt_Thickness
            // 
            resources.ApplyResources(this.txt_Thickness, "txt_Thickness");
            this.txt_Thickness.Maximum = new decimal(new int[] {
            32727,
            0,
            0,
            0});
            this.txt_Thickness.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Thickness.Name = "txt_Thickness";
            this.txt_Thickness.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Thickness.ValueChanged += new System.EventHandler(this.txt_Thickness_ValueChanged);
            // 
            // txt_Filter
            // 
            resources.ApplyResources(this.txt_Filter, "txt_Filter");
            this.txt_Filter.Maximum = new decimal(new int[] {
            32727,
            0,
            0,
            0});
            this.txt_Filter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Filter.Name = "txt_Filter";
            this.txt_Filter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Filter.ValueChanged += new System.EventHandler(this.txt_Filter_ValueChanged);
            // 
            // txt_SizeTolerance
            // 
            resources.ApplyResources(this.txt_SizeTolerance, "txt_SizeTolerance");
            this.txt_SizeTolerance.Name = "txt_SizeTolerance";
            this.txt_SizeTolerance.ValueChanged += new System.EventHandler(this.txt_SizeTolerance_ValueChanged);
            // 
            // srmlabel100
            // 
            resources.ApplyResources(this.srmlabel100, "srmlabel100");
            this.srmlabel100.Name = "srmlabel100";
            this.srmlabel100.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_TransType
            // 
            this.cbo_TransType.BackColor = System.Drawing.Color.White;
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
            // txt_Tolerance
            // 
            resources.ApplyResources(this.txt_Tolerance, "txt_Tolerance");
            this.txt_Tolerance.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.txt_Tolerance.Name = "txt_Tolerance";
            this.txt_Tolerance.ValueChanged += new System.EventHandler(this.txt_Tolerance_ValueChanged);
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_FilteringPass
            // 
            resources.ApplyResources(this.txt_FilteringPass, "txt_FilteringPass");
            this.txt_FilteringPass.Maximum = new decimal(new int[] {
            32727,
            0,
            0,
            0});
            this.txt_FilteringPass.Name = "txt_FilteringPass";
            this.txt_FilteringPass.ValueChanged += new System.EventHandler(this.txt_FilteringPass_ValueChanged);
            // 
            // txt_FilteringThreshold
            // 
            this.txt_FilteringThreshold.DecimalPlaces = 1;
            this.txt_FilteringThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.txt_FilteringThreshold, "txt_FilteringThreshold");
            this.txt_FilteringThreshold.Maximum = new decimal(new int[] {
            32727,
            0,
            0,
            0});
            this.txt_FilteringThreshold.Name = "txt_FilteringThreshold";
            this.txt_FilteringThreshold.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.txt_FilteringThreshold.ValueChanged += new System.EventHandler(this.txt_FilteringThreshold_ValueChanged);
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
            // txt_FittingSamplingStep
            // 
            resources.ApplyResources(this.txt_FittingSamplingStep, "txt_FittingSamplingStep");
            this.txt_FittingSamplingStep.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_FittingSamplingStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_FittingSamplingStep.Name = "txt_FittingSamplingStep";
            this.txt_FittingSamplingStep.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.txt_FittingSamplingStep.ValueChanged += new System.EventHandler(this.txt_FittingSamplingStep_ValueChanged);
            // 
            // srmLabel6
            // 
            resources.ApplyResources(this.srmLabel6, "srmLabel6");
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // AdvancedRGaugeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.txt_FittingSamplingStep);
            this.Controls.Add(this.srmLabel6);
            this.Controls.Add(this.gb_GainSetting);
            this.Controls.Add(this.txt_FilteringThreshold);
            this.Controls.Add(this.txt_FilteringPass);
            this.Controls.Add(this.srmLabel4);
            this.Controls.Add(this.srmLabel5);
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.txt_Tolerance);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.cbo_TransType);
            this.Controls.Add(this.txt_SizeTolerance);
            this.Controls.Add(this.srmlabel100);
            this.Controls.Add(this.txt_Filter);
            this.Controls.Add(this.txt_Thickness);
            this.Controls.Add(this.txt_MinArea);
            this.Controls.Add(this.txt_MinAmplitude);
            this.Controls.Add(this.txt_threshold);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.srmLabel8);
            this.Controls.Add(this.cbo_TransChoice);
            this.Controls.Add(this.srmLabel13);
            this.Controls.Add(this.srmLabel12);
            this.Controls.Add(this.srmLabel11);
            this.Controls.Add(this.srmLabel10);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdvancedRGaugeForm";
            this.Load += new System.EventHandler(this.AdvancedRGaugeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txt_threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_MinAmplitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_MinArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Thickness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Filter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_SizeTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Tolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_FilteringPass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_FilteringThreshold)).EndInit();
            this.gb_GainSetting.ResumeLayout(false);
            this.gb_GainSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Gain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_FittingSamplingStep)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMComboBox cbo_TransChoice;
        private SRMControl.SRMLabel srmLabel13;
        private SRMControl.SRMLabel srmLabel12;
        private SRMControl.SRMLabel srmLabel11;
        private SRMControl.SRMLabel srmLabel10;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMLabel srmLabel1;
        private System.Windows.Forms.NumericUpDown txt_threshold;
        private System.Windows.Forms.NumericUpDown txt_MinAmplitude;
        private System.Windows.Forms.NumericUpDown txt_MinArea;
        private System.Windows.Forms.NumericUpDown txt_Thickness;
        private System.Windows.Forms.NumericUpDown txt_Filter;
        private System.Windows.Forms.NumericUpDown txt_SizeTolerance;
        private SRMControl.SRMLabel srmlabel100;
        private System.Windows.Forms.Label label4;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMComboBox cbo_TransType;
        private System.Windows.Forms.NumericUpDown txt_Tolerance;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMLabel srmLabel5;
        private System.Windows.Forms.NumericUpDown txt_FilteringPass;
        private System.Windows.Forms.NumericUpDown txt_FilteringThreshold;
        private SRMControl.SRMGroupBox gb_GainSetting;
        private SRMControl.SRMInputBox txt_GainValue;
        private System.Windows.Forms.TrackBar trackBar_Gain;
        private System.Windows.Forms.NumericUpDown txt_FittingSamplingStep;
        private SRMControl.SRMLabel srmLabel6;
    }
}