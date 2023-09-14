namespace VisionModule
{
    partial class VisionGRRPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisionGRRPage));
            this.group_InspectionCounter = new SRMControl.SRMGroupBox();
            this.lbl_OperatorsCounter = new SRMControl.SRMLabel();
            this.lbl_TrialsCounter = new SRMControl.SRMLabel();
            this.lbl_PartsCounter = new SRMControl.SRMLabel();
            this.lbl_Operators2 = new SRMControl.SRMLabel();
            this.lbl_Parts2 = new SRMControl.SRMLabel();
            this.lbl_Trials2 = new SRMControl.SRMLabel();
            this.group_GRRSetting = new SRMControl.SRMGroupBox();
            this.txt_Trials = new SRMControl.SRMInputBox();
            this.txt_Operators = new SRMControl.SRMInputBox();
            this.txt_Parts = new SRMControl.SRMInputBox();
            this.lbl_Operators1 = new SRMControl.SRMLabel();
            this.lbl_Parts1 = new SRMControl.SRMLabel();
            this.lbl_Trials1 = new SRMControl.SRMLabel();
            this.group_GRRMode = new SRMControl.SRMGroupBox();
            this.radioBtn_Dynamic = new SRMControl.SRMRadioButton();
            this.radioBtn_Static = new SRMControl.SRMRadioButton();
            this.timer_GRR = new System.Windows.Forms.Timer(this.components);
            this.lbl_Unit1 = new System.Windows.Forms.Label();
            this.lbl_ResultStatus1 = new SRMControl.SRMLabel();
            this.lbl_ResultStatus2 = new SRMControl.SRMLabel();
            this.lbl_Unit2 = new System.Windows.Forms.Label();
            this.group_InspectionCounter.SuspendLayout();
            this.group_GRRSetting.SuspendLayout();
            this.group_GRRMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_InspectionCounter
            // 
            resources.ApplyResources(this.group_InspectionCounter, "group_InspectionCounter");
            this.group_InspectionCounter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_InspectionCounter.Controls.Add(this.lbl_OperatorsCounter);
            this.group_InspectionCounter.Controls.Add(this.lbl_TrialsCounter);
            this.group_InspectionCounter.Controls.Add(this.lbl_PartsCounter);
            this.group_InspectionCounter.Controls.Add(this.lbl_Operators2);
            this.group_InspectionCounter.Controls.Add(this.lbl_Parts2);
            this.group_InspectionCounter.Controls.Add(this.lbl_Trials2);
            this.group_InspectionCounter.Name = "group_InspectionCounter";
            this.group_InspectionCounter.TabStop = false;
            this.group_InspectionCounter.Enter += new System.EventHandler(this.group_InspectionCounter_Enter);
            // 
            // lbl_OperatorsCounter
            // 
            resources.ApplyResources(this.lbl_OperatorsCounter, "lbl_OperatorsCounter");
            this.lbl_OperatorsCounter.BackColor = System.Drawing.Color.PaleTurquoise;
            this.lbl_OperatorsCounter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_OperatorsCounter.Name = "lbl_OperatorsCounter";
            this.lbl_OperatorsCounter.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TrialsCounter
            // 
            resources.ApplyResources(this.lbl_TrialsCounter, "lbl_TrialsCounter");
            this.lbl_TrialsCounter.BackColor = System.Drawing.Color.PaleTurquoise;
            this.lbl_TrialsCounter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_TrialsCounter.Name = "lbl_TrialsCounter";
            this.lbl_TrialsCounter.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PartsCounter
            // 
            resources.ApplyResources(this.lbl_PartsCounter, "lbl_PartsCounter");
            this.lbl_PartsCounter.BackColor = System.Drawing.Color.PaleTurquoise;
            this.lbl_PartsCounter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_PartsCounter.Name = "lbl_PartsCounter";
            this.lbl_PartsCounter.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Operators2
            // 
            resources.ApplyResources(this.lbl_Operators2, "lbl_Operators2");
            this.lbl_Operators2.Name = "lbl_Operators2";
            this.lbl_Operators2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Parts2
            // 
            resources.ApplyResources(this.lbl_Parts2, "lbl_Parts2");
            this.lbl_Parts2.Name = "lbl_Parts2";
            this.lbl_Parts2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Trials2
            // 
            resources.ApplyResources(this.lbl_Trials2, "lbl_Trials2");
            this.lbl_Trials2.Name = "lbl_Trials2";
            this.lbl_Trials2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // group_GRRSetting
            // 
            resources.ApplyResources(this.group_GRRSetting, "group_GRRSetting");
            this.group_GRRSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_GRRSetting.Controls.Add(this.txt_Trials);
            this.group_GRRSetting.Controls.Add(this.txt_Operators);
            this.group_GRRSetting.Controls.Add(this.txt_Parts);
            this.group_GRRSetting.Controls.Add(this.lbl_Operators1);
            this.group_GRRSetting.Controls.Add(this.lbl_Parts1);
            this.group_GRRSetting.Controls.Add(this.lbl_Trials1);
            this.group_GRRSetting.Name = "group_GRRSetting";
            this.group_GRRSetting.TabStop = false;
            // 
            // txt_Trials
            // 
            resources.ApplyResources(this.txt_Trials, "txt_Trials");
            this.txt_Trials.BackColor = System.Drawing.Color.White;
            this.txt_Trials.DecimalPlaces = 0;
            this.txt_Trials.DecMaxValue = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.txt_Trials.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Trials.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Trials.ForeColor = System.Drawing.Color.Black;
            this.txt_Trials.InputType = SRMControl.InputType.Number;
            this.txt_Trials.Name = "txt_Trials";
            this.txt_Trials.NormalBackColor = System.Drawing.Color.White;
            this.txt_Trials.TextChanged += new System.EventHandler(this.txt_CounterSetting_TextChanged);
            // 
            // txt_Operators
            // 
            resources.ApplyResources(this.txt_Operators, "txt_Operators");
            this.txt_Operators.BackColor = System.Drawing.Color.White;
            this.txt_Operators.DecimalPlaces = 0;
            this.txt_Operators.DecMaxValue = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.txt_Operators.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Operators.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Operators.ForeColor = System.Drawing.Color.Black;
            this.txt_Operators.InputType = SRMControl.InputType.Number;
            this.txt_Operators.Name = "txt_Operators";
            this.txt_Operators.NormalBackColor = System.Drawing.Color.White;
            this.txt_Operators.TextChanged += new System.EventHandler(this.txt_CounterSetting_TextChanged);
            // 
            // txt_Parts
            // 
            resources.ApplyResources(this.txt_Parts, "txt_Parts");
            this.txt_Parts.BackColor = System.Drawing.Color.White;
            this.txt_Parts.DecimalPlaces = 0;
            this.txt_Parts.DecMaxValue = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.txt_Parts.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Parts.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Parts.ForeColor = System.Drawing.Color.Black;
            this.txt_Parts.InputType = SRMControl.InputType.Number;
            this.txt_Parts.Name = "txt_Parts";
            this.txt_Parts.NormalBackColor = System.Drawing.Color.White;
            this.txt_Parts.TextChanged += new System.EventHandler(this.txt_CounterSetting_TextChanged);
            this.txt_Parts.Leave += new System.EventHandler(this.txt_Parts_Leave);
            // 
            // lbl_Operators1
            // 
            resources.ApplyResources(this.lbl_Operators1, "lbl_Operators1");
            this.lbl_Operators1.Name = "lbl_Operators1";
            this.lbl_Operators1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Parts1
            // 
            resources.ApplyResources(this.lbl_Parts1, "lbl_Parts1");
            this.lbl_Parts1.Name = "lbl_Parts1";
            this.lbl_Parts1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Trials1
            // 
            resources.ApplyResources(this.lbl_Trials1, "lbl_Trials1");
            this.lbl_Trials1.Name = "lbl_Trials1";
            this.lbl_Trials1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // group_GRRMode
            // 
            resources.ApplyResources(this.group_GRRMode, "group_GRRMode");
            this.group_GRRMode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_GRRMode.Controls.Add(this.radioBtn_Dynamic);
            this.group_GRRMode.Controls.Add(this.radioBtn_Static);
            this.group_GRRMode.Name = "group_GRRMode";
            this.group_GRRMode.TabStop = false;
            // 
            // radioBtn_Dynamic
            // 
            resources.ApplyResources(this.radioBtn_Dynamic, "radioBtn_Dynamic");
            this.radioBtn_Dynamic.Name = "radioBtn_Dynamic";
            this.radioBtn_Dynamic.UseVisualStyleBackColor = true;
            // 
            // radioBtn_Static
            // 
            resources.ApplyResources(this.radioBtn_Static, "radioBtn_Static");
            this.radioBtn_Static.Checked = true;
            this.radioBtn_Static.Name = "radioBtn_Static";
            this.radioBtn_Static.TabStop = true;
            this.radioBtn_Static.UseVisualStyleBackColor = true;
            // 
            // timer_GRR
            // 
            this.timer_GRR.Enabled = true;
            this.timer_GRR.Interval = 10;
            this.timer_GRR.Tick += new System.EventHandler(this.timer_GRR_Tick);
            // 
            // lbl_Unit1
            // 
            resources.ApplyResources(this.lbl_Unit1, "lbl_Unit1");
            this.lbl_Unit1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_Unit1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Unit1.ForeColor = System.Drawing.Color.Navy;
            this.lbl_Unit1.Name = "lbl_Unit1";
            // 
            // lbl_ResultStatus1
            // 
            resources.ApplyResources(this.lbl_ResultStatus1, "lbl_ResultStatus1");
            this.lbl_ResultStatus1.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.lbl_ResultStatus1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ResultStatus1.ForeColor = System.Drawing.Color.Black;
            this.lbl_ResultStatus1.Name = "lbl_ResultStatus1";
            this.lbl_ResultStatus1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_ResultStatus2
            // 
            resources.ApplyResources(this.lbl_ResultStatus2, "lbl_ResultStatus2");
            this.lbl_ResultStatus2.BackColor = System.Drawing.Color.White;
            this.lbl_ResultStatus2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ResultStatus2.ForeColor = System.Drawing.Color.Black;
            this.lbl_ResultStatus2.Name = "lbl_ResultStatus2";
            this.lbl_ResultStatus2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Unit2
            // 
            resources.ApplyResources(this.lbl_Unit2, "lbl_Unit2");
            this.lbl_Unit2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.lbl_Unit2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_Unit2.ForeColor = System.Drawing.Color.Navy;
            this.lbl_Unit2.Name = "lbl_Unit2";
            // 
            // VisionGRRPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.lbl_Unit1);
            this.Controls.Add(this.lbl_ResultStatus1);
            this.Controls.Add(this.group_InspectionCounter);
            this.Controls.Add(this.lbl_ResultStatus2);
            this.Controls.Add(this.group_GRRSetting);
            this.Controls.Add(this.lbl_Unit2);
            this.Controls.Add(this.group_GRRMode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "VisionGRRPage";
            this.group_InspectionCounter.ResumeLayout(false);
            this.group_InspectionCounter.PerformLayout();
            this.group_GRRSetting.ResumeLayout(false);
            this.group_GRRSetting.PerformLayout();
            this.group_GRRMode.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMGroupBox group_InspectionCounter;
        private SRMControl.SRMLabel lbl_OperatorsCounter;
        private SRMControl.SRMLabel lbl_TrialsCounter;
        private SRMControl.SRMLabel lbl_PartsCounter;
        private SRMControl.SRMLabel lbl_Operators2;
        private SRMControl.SRMLabel lbl_Parts2;
        private SRMControl.SRMLabel lbl_Trials2;
        private SRMControl.SRMGroupBox group_GRRSetting;
        private SRMControl.SRMInputBox txt_Trials;
        private SRMControl.SRMInputBox txt_Operators;
        private SRMControl.SRMInputBox txt_Parts;
        private SRMControl.SRMLabel lbl_Operators1;
        private SRMControl.SRMLabel lbl_Parts1;
        private SRMControl.SRMLabel lbl_Trials1;
        private SRMControl.SRMGroupBox group_GRRMode;
        private SRMControl.SRMRadioButton radioBtn_Dynamic;
        private SRMControl.SRMRadioButton radioBtn_Static;
        private System.Windows.Forms.Timer timer_GRR;
        private System.Windows.Forms.Label lbl_Unit1;
        private SRMControl.SRMLabel lbl_ResultStatus1;
        private SRMControl.SRMLabel lbl_ResultStatus2;
        private System.Windows.Forms.Label lbl_Unit2;
    }
}