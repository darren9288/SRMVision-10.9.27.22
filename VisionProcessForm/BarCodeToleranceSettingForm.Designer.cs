namespace VisionProcessForm
{
    partial class BarcodeToleranceSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BarcodeToleranceSettingForm));
            this.lbl_Pin1ROI = new SRMControl.SRMLabel();
            this.tab_VisionControl = new SRMControl.SRMTabControl();
            this.tp_Barcode = new System.Windows.Forms.TabPage();
            this.gb_Barcode = new SRMControl.SRMGroupBox();
            this.txt_BarcodeAngleRange = new System.Windows.Forms.NumericUpDown();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.txt_GainRange = new System.Windows.Forms.NumericUpDown();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.tp_Pattern = new System.Windows.Forms.TabPage();
            this.gb_PatternSetting = new SRMControl.SRMGroupBox();
            this.txt_PatternAngleRange = new System.Windows.Forms.NumericUpDown();
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.gb_PatternScoreSetting = new SRMControl.SRMGroupBox();
            this.txt_ScoreTolerance = new SRMControl.SRMInputBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.trackBar_ScoreTolerance = new System.Windows.Forms.TrackBar();
            this.lbl_ScorePercent = new SRMControl.SRMLabel();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.lbl_Score = new SRMControl.SRMLabel();
            this.lbl_ScoreTitle = new SRMControl.SRMLabel();
            this.btn_Close = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tab_VisionControl.SuspendLayout();
            this.tp_Barcode.SuspendLayout();
            this.gb_Barcode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_BarcodeAngleRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_GainRange)).BeginInit();
            this.tp_Pattern.SuspendLayout();
            this.gb_PatternSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_PatternAngleRange)).BeginInit();
            this.gb_PatternScoreSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ScoreTolerance)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Pin1ROI
            // 
            resources.ApplyResources(this.lbl_Pin1ROI, "lbl_Pin1ROI");
            this.lbl_Pin1ROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_Pin1ROI.ForeColor = System.Drawing.Color.Black;
            this.lbl_Pin1ROI.Name = "lbl_Pin1ROI";
            this.lbl_Pin1ROI.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tab_VisionControl
            // 
            resources.ApplyResources(this.tab_VisionControl, "tab_VisionControl");
            this.tab_VisionControl.Controls.Add(this.tp_Barcode);
            this.tab_VisionControl.Controls.Add(this.tp_Pattern);
            this.tab_VisionControl.Name = "tab_VisionControl";
            this.tab_VisionControl.SelectedIndex = 0;
            // 
            // tp_Barcode
            // 
            resources.ApplyResources(this.tp_Barcode, "tp_Barcode");
            this.tp_Barcode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Barcode.Controls.Add(this.gb_Barcode);
            this.tp_Barcode.Name = "tp_Barcode";
            // 
            // gb_Barcode
            // 
            resources.ApplyResources(this.gb_Barcode, "gb_Barcode");
            this.gb_Barcode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.gb_Barcode.Controls.Add(this.txt_BarcodeAngleRange);
            this.gb_Barcode.Controls.Add(this.srmLabel3);
            this.gb_Barcode.Controls.Add(this.txt_GainRange);
            this.gb_Barcode.Controls.Add(this.srmLabel4);
            this.gb_Barcode.Name = "gb_Barcode";
            this.gb_Barcode.TabStop = false;
            // 
            // txt_BarcodeAngleRange
            // 
            resources.ApplyResources(this.txt_BarcodeAngleRange, "txt_BarcodeAngleRange");
            this.txt_BarcodeAngleRange.DecimalPlaces = 1;
            this.txt_BarcodeAngleRange.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.txt_BarcodeAngleRange.Name = "txt_BarcodeAngleRange";
            this.txt_BarcodeAngleRange.ValueChanged += new System.EventHandler(this.txt_BarcodeAngleRange_ValueChanged);
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_GainRange
            // 
            resources.ApplyResources(this.txt_GainRange, "txt_GainRange");
            this.txt_GainRange.DecimalPlaces = 1;
            this.txt_GainRange.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.txt_GainRange.Name = "txt_GainRange";
            this.txt_GainRange.ValueChanged += new System.EventHandler(this.txt_GainRange_ValueChanged);
            // 
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // tp_Pattern
            // 
            resources.ApplyResources(this.tp_Pattern, "tp_Pattern");
            this.tp_Pattern.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Pattern.Controls.Add(this.gb_PatternSetting);
            this.tp_Pattern.Controls.Add(this.gb_PatternScoreSetting);
            this.tp_Pattern.Name = "tp_Pattern";
            // 
            // gb_PatternSetting
            // 
            resources.ApplyResources(this.gb_PatternSetting, "gb_PatternSetting");
            this.gb_PatternSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.gb_PatternSetting.Controls.Add(this.txt_PatternAngleRange);
            this.gb_PatternSetting.Controls.Add(this.srmLabel5);
            this.gb_PatternSetting.Name = "gb_PatternSetting";
            this.gb_PatternSetting.TabStop = false;
            // 
            // txt_PatternAngleRange
            // 
            resources.ApplyResources(this.txt_PatternAngleRange, "txt_PatternAngleRange");
            this.txt_PatternAngleRange.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.txt_PatternAngleRange.Name = "txt_PatternAngleRange";
            this.txt_PatternAngleRange.ValueChanged += new System.EventHandler(this.txt_PatternAngleRange_ValueChanged);
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // gb_PatternScoreSetting
            // 
            resources.ApplyResources(this.gb_PatternScoreSetting, "gb_PatternScoreSetting");
            this.gb_PatternScoreSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.gb_PatternScoreSetting.Controls.Add(this.txt_ScoreTolerance);
            this.gb_PatternScoreSetting.Controls.Add(this.srmLabel1);
            this.gb_PatternScoreSetting.Controls.Add(this.trackBar_ScoreTolerance);
            this.gb_PatternScoreSetting.Controls.Add(this.lbl_ScorePercent);
            this.gb_PatternScoreSetting.Controls.Add(this.srmLabel2);
            this.gb_PatternScoreSetting.Controls.Add(this.lbl_Score);
            this.gb_PatternScoreSetting.Controls.Add(this.lbl_ScoreTitle);
            this.gb_PatternScoreSetting.Name = "gb_PatternScoreSetting";
            this.gb_PatternScoreSetting.TabStop = false;
            // 
            // txt_ScoreTolerance
            // 
            resources.ApplyResources(this.txt_ScoreTolerance, "txt_ScoreTolerance");
            this.txt_ScoreTolerance.BackColor = System.Drawing.Color.White;
            this.txt_ScoreTolerance.DecimalPlaces = 0;
            this.txt_ScoreTolerance.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_ScoreTolerance.DecMinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_ScoreTolerance.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_ScoreTolerance.ForeColor = System.Drawing.Color.Black;
            this.txt_ScoreTolerance.InputType = SRMControl.InputType.Number;
            this.txt_ScoreTolerance.Name = "txt_ScoreTolerance";
            this.txt_ScoreTolerance.NormalBackColor = System.Drawing.Color.White;
            this.txt_ScoreTolerance.TextChanged += new System.EventHandler(this.txt_ScoreTolerance_TextChanged);
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // trackBar_ScoreTolerance
            // 
            resources.ApplyResources(this.trackBar_ScoreTolerance, "trackBar_ScoreTolerance");
            this.trackBar_ScoreTolerance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_ScoreTolerance.LargeChange = 1;
            this.trackBar_ScoreTolerance.Maximum = 100;
            this.trackBar_ScoreTolerance.Minimum = 1;
            this.trackBar_ScoreTolerance.Name = "trackBar_ScoreTolerance";
            this.trackBar_ScoreTolerance.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_ScoreTolerance.Value = 1;
            this.trackBar_ScoreTolerance.Scroll += new System.EventHandler(this.trackBar_ScoreTolerance_Scroll);
            // 
            // lbl_ScorePercent
            // 
            resources.ApplyResources(this.lbl_ScorePercent, "lbl_ScorePercent");
            this.lbl_ScorePercent.Name = "lbl_ScorePercent";
            this.lbl_ScorePercent.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Score
            // 
            resources.ApplyResources(this.lbl_Score, "lbl_Score");
            this.lbl_Score.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Score.Name = "lbl_Score";
            this.lbl_Score.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ScoreTitle
            // 
            resources.ApplyResources(this.lbl_ScoreTitle, "lbl_ScoreTitle");
            this.lbl_ScoreTitle.Name = "lbl_ScoreTitle";
            this.lbl_ScoreTitle.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Close
            // 
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // BarcodeToleranceSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.lbl_Pin1ROI);
            this.Controls.Add(this.tab_VisionControl);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BarcodeToleranceSettingForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BarcodeToleranceSettingForm_FormClosing);
            this.Load += new System.EventHandler(this.BarcodeToleranceSettingForm_Load);
            this.tab_VisionControl.ResumeLayout(false);
            this.tp_Barcode.ResumeLayout(false);
            this.gb_Barcode.ResumeLayout(false);
            this.gb_Barcode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_BarcodeAngleRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_GainRange)).EndInit();
            this.tp_Pattern.ResumeLayout(false);
            this.gb_PatternSetting.ResumeLayout(false);
            this.gb_PatternSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_PatternAngleRange)).EndInit();
            this.gb_PatternScoreSetting.ResumeLayout(false);
            this.gb_PatternScoreSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ScoreTolerance)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMLabel lbl_Pin1ROI;
        private SRMControl.SRMTabControl tab_VisionControl;
        private System.Windows.Forms.TabPage tp_Barcode;
        private System.Windows.Forms.TabPage tp_Pattern;
        private SRMControl.SRMGroupBox gb_PatternScoreSetting;
        private SRMControl.SRMInputBox txt_ScoreTolerance;
        private SRMControl.SRMLabel srmLabel1;
        private System.Windows.Forms.TrackBar trackBar_ScoreTolerance;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel lbl_ScorePercent;
        private SRMControl.SRMLabel lbl_Score;
        private SRMControl.SRMLabel lbl_ScoreTitle;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMGroupBox gb_Barcode;
        private System.Windows.Forms.NumericUpDown txt_BarcodeAngleRange;
        private SRMControl.SRMLabel srmLabel3;
        private System.Windows.Forms.NumericUpDown txt_GainRange;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMGroupBox gb_PatternSetting;
        private System.Windows.Forms.NumericUpDown txt_PatternAngleRange;
        private SRMControl.SRMLabel srmLabel5;
        private System.Windows.Forms.Timer timer1;
    }
}