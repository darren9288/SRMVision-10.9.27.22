namespace VisionProcessForm
{
    partial class YieldSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YieldSettingForm));
            this.txt_LowYield = new SRMControl.SRMInputBox();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.chk_StopLowYield = new SRMControl.SRMCheckBox();
            this.srmLabel6 = new SRMControl.SRMLabel();
            this.txt_MinUnitCheck = new SRMControl.SRMInputBox();
            this.chk_StopContinuousPass = new SRMControl.SRMCheckBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.txt_MinPassUnit = new SRMControl.SRMInputBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.txt_MinFailUnit = new SRMControl.SRMInputBox();
            this.chk_StopContinuousFail = new SRMControl.SRMCheckBox();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.srmLabel5 = new SRMControl.SRMLabel();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.txt_DelayCheckIO = new SRMControl.SRMInputBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_LowYield
            // 
            resources.ApplyResources(this.txt_LowYield, "txt_LowYield");
            this.txt_LowYield.BackColor = System.Drawing.Color.White;
            this.txt_LowYield.DataType = SRMControl.SRMDataType.Int32;
            this.txt_LowYield.DecimalPlaces = 1;
            this.txt_LowYield.DecMaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txt_LowYield.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_LowYield.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_LowYield.ForeColor = System.Drawing.Color.Black;
            this.txt_LowYield.InputType = SRMControl.InputType.Number;
            this.txt_LowYield.Name = "txt_LowYield";
            this.txt_LowYield.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // chk_StopLowYield
            // 
            resources.ApplyResources(this.chk_StopLowYield, "chk_StopLowYield");
            this.chk_StopLowYield.CheckedColor = System.Drawing.Color.Empty;
            this.chk_StopLowYield.Name = "chk_StopLowYield";
            this.chk_StopLowYield.Selected = false;
            this.chk_StopLowYield.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_StopLowYield.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // srmLabel6
            // 
            resources.ApplyResources(this.srmLabel6, "srmLabel6");
            this.srmLabel6.Name = "srmLabel6";
            this.srmLabel6.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MinUnitCheck
            // 
            resources.ApplyResources(this.txt_MinUnitCheck, "txt_MinUnitCheck");
            this.txt_MinUnitCheck.BackColor = System.Drawing.Color.White;
            this.txt_MinUnitCheck.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MinUnitCheck.DecimalPlaces = 0;
            this.txt_MinUnitCheck.DecMaxValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.txt_MinUnitCheck.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MinUnitCheck.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MinUnitCheck.ForeColor = System.Drawing.Color.Black;
            this.txt_MinUnitCheck.InputType = SRMControl.InputType.Number;
            this.txt_MinUnitCheck.Name = "txt_MinUnitCheck";
            this.txt_MinUnitCheck.NormalBackColor = System.Drawing.Color.White;
            // 
            // chk_StopContinuousPass
            // 
            resources.ApplyResources(this.chk_StopContinuousPass, "chk_StopContinuousPass");
            this.chk_StopContinuousPass.CheckedColor = System.Drawing.Color.Empty;
            this.chk_StopContinuousPass.Name = "chk_StopContinuousPass";
            this.chk_StopContinuousPass.Selected = false;
            this.chk_StopContinuousPass.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_StopContinuousPass.UnCheckedColor = System.Drawing.Color.Empty;
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MinPassUnit
            // 
            resources.ApplyResources(this.txt_MinPassUnit, "txt_MinPassUnit");
            this.txt_MinPassUnit.BackColor = System.Drawing.Color.White;
            this.txt_MinPassUnit.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MinPassUnit.DecimalPlaces = 0;
            this.txt_MinPassUnit.DecMaxValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.txt_MinPassUnit.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MinPassUnit.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MinPassUnit.ForeColor = System.Drawing.Color.Black;
            this.txt_MinPassUnit.InputType = SRMControl.InputType.Number;
            this.txt_MinPassUnit.Name = "txt_MinPassUnit";
            this.txt_MinPassUnit.NormalBackColor = System.Drawing.Color.White;
            this.txt_MinPassUnit.TextChanged += new System.EventHandler(this.txt_MinPassUnit_TextChanged);
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MinFailUnit
            // 
            resources.ApplyResources(this.txt_MinFailUnit, "txt_MinFailUnit");
            this.txt_MinFailUnit.BackColor = System.Drawing.Color.White;
            this.txt_MinFailUnit.DataType = SRMControl.SRMDataType.Int32;
            this.txt_MinFailUnit.DecimalPlaces = 0;
            this.txt_MinFailUnit.DecMaxValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.txt_MinFailUnit.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MinFailUnit.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MinFailUnit.ForeColor = System.Drawing.Color.Black;
            this.txt_MinFailUnit.InputType = SRMControl.InputType.Number;
            this.txt_MinFailUnit.Name = "txt_MinFailUnit";
            this.txt_MinFailUnit.NormalBackColor = System.Drawing.Color.White;
            this.txt_MinFailUnit.TextChanged += new System.EventHandler(this.txt_MinFailUnit_TextChanged);
            // 
            // chk_StopContinuousFail
            // 
            resources.ApplyResources(this.chk_StopContinuousFail, "chk_StopContinuousFail");
            this.chk_StopContinuousFail.CheckedColor = System.Drawing.Color.Empty;
            this.chk_StopContinuousFail.Name = "chk_StopContinuousFail";
            this.chk_StopContinuousFail.Selected = false;
            this.chk_StopContinuousFail.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_StopContinuousFail.UnCheckedColor = System.Drawing.Color.Empty;
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
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.srmLabel1);
            this.groupBox1.Controls.Add(this.chk_StopContinuousPass);
            this.groupBox1.Controls.Add(this.srmLabel3);
            this.groupBox1.Controls.Add(this.txt_MinPassUnit);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.srmLabel2);
            this.groupBox2.Controls.Add(this.chk_StopContinuousFail);
            this.groupBox2.Controls.Add(this.srmLabel4);
            this.groupBox2.Controls.Add(this.txt_MinFailUnit);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.srmLabel7);
            this.groupBox3.Controls.Add(this.txt_LowYield);
            this.groupBox3.Controls.Add(this.chk_StopLowYield);
            this.groupBox3.Controls.Add(this.srmLabel6);
            this.groupBox3.Controls.Add(this.txt_MinUnitCheck);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // srmLabel5
            // 
            resources.ApplyResources(this.srmLabel5, "srmLabel5");
            this.srmLabel5.Name = "srmLabel5";
            this.srmLabel5.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_DelayCheckIO
            // 
            resources.ApplyResources(this.txt_DelayCheckIO, "txt_DelayCheckIO");
            this.txt_DelayCheckIO.BackColor = System.Drawing.Color.White;
            this.txt_DelayCheckIO.DataType = SRMControl.SRMDataType.Int32;
            this.txt_DelayCheckIO.DecimalPlaces = 0;
            this.txt_DelayCheckIO.DecMaxValue = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.txt_DelayCheckIO.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_DelayCheckIO.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_DelayCheckIO.ForeColor = System.Drawing.Color.Black;
            this.txt_DelayCheckIO.InputType = SRMControl.InputType.Number;
            this.txt_DelayCheckIO.Name = "txt_DelayCheckIO";
            this.txt_DelayCheckIO.NormalBackColor = System.Drawing.Color.White;
            // 
            // YieldSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.srmLabel5);
            this.Controls.Add(this.srmLabel8);
            this.Controls.Add(this.txt_DelayCheckIO);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "YieldSettingForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.YieldSettingForm_FormClosing);
            this.Load += new System.EventHandler(this.YieldSettingForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMInputBox txt_LowYield;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMCheckBox chk_StopLowYield;
        private SRMControl.SRMLabel srmLabel6;
        private SRMControl.SRMInputBox txt_MinUnitCheck;
        private SRMControl.SRMCheckBox chk_StopContinuousPass;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMInputBox txt_MinPassUnit;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMInputBox txt_MinFailUnit;
        private SRMControl.SRMCheckBox chk_StopContinuousFail;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel srmLabel4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Timer timer1;
        private SRMControl.SRMLabel srmLabel5;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMInputBox txt_DelayCheckIO;
    }
}