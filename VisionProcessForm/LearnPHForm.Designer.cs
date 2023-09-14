namespace VisionProcessForm
{
    partial class LearnPHForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearnPHForm));
            this.tabCtrl_Setup = new SRMControl.SRMTabControl();
            this.tp_Step1 = new System.Windows.Forms.TabPage();
            this.srmGroupBox2 = new SRMControl.SRMGroupBox();
            this.txt_MinArea = new SRMControl.SRMInputBox();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.lbl_PHBlackArea = new System.Windows.Forms.Label();
            this.srmLabel17 = new SRMControl.SRMLabel();
            this.txt_MinBlackArea = new SRMControl.SRMInputBox();
            this.srmLabel16 = new SRMControl.SRMLabel();
            this.btn_PHThreshold = new SRMControl.SRMButton();
            this.lbl_PHThreshold = new System.Windows.Forms.Label();
            this.srmLabel26 = new SRMControl.SRMLabel();
            this.btn_SavePH = new SRMControl.SRMButton();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.srmLabel15 = new SRMControl.SRMLabel();
            this.lbl_TitleStepOrientROI = new SRMControl.SRMLabel();
            this.lbl_TitleStepDontCareArea = new SRMControl.SRMLabel();
            this.lbl_TitleStepPocket = new SRMControl.SRMLabel();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.lbl_StepNo = new SRMControl.SRMLabel();
            this.lbl_TitleStep1 = new SRMControl.SRMLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabCtrl_Setup.SuspendLayout();
            this.tp_Step1.SuspendLayout();
            this.srmGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            this.SuspendLayout();
            // 
            // tabCtrl_Setup
            // 
            resources.ApplyResources(this.tabCtrl_Setup, "tabCtrl_Setup");
            this.tabCtrl_Setup.Controls.Add(this.tp_Step1);
            this.tabCtrl_Setup.Name = "tabCtrl_Setup";
            this.tabCtrl_Setup.SelectedIndex = 0;
            this.tabCtrl_Setup.TabStop = false;
            // 
            // tp_Step1
            // 
            resources.ApplyResources(this.tp_Step1, "tp_Step1");
            this.tp_Step1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_Step1.Controls.Add(this.srmGroupBox2);
            this.tp_Step1.Controls.Add(this.btn_SavePH);
            this.tp_Step1.Controls.Add(this.pictureBox10);
            this.tp_Step1.Controls.Add(this.srmLabel15);
            this.tp_Step1.Name = "tp_Step1";
            // 
            // srmGroupBox2
            // 
            resources.ApplyResources(this.srmGroupBox2, "srmGroupBox2");
            this.srmGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox2.Controls.Add(this.txt_MinArea);
            this.srmGroupBox2.Controls.Add(this.srmLabel4);
            this.srmGroupBox2.Controls.Add(this.lbl_PHBlackArea);
            this.srmGroupBox2.Controls.Add(this.srmLabel17);
            this.srmGroupBox2.Controls.Add(this.txt_MinBlackArea);
            this.srmGroupBox2.Controls.Add(this.srmLabel16);
            this.srmGroupBox2.Controls.Add(this.btn_PHThreshold);
            this.srmGroupBox2.Controls.Add(this.lbl_PHThreshold);
            this.srmGroupBox2.Controls.Add(this.srmLabel26);
            this.srmGroupBox2.Name = "srmGroupBox2";
            this.srmGroupBox2.TabStop = false;
            // 
            // txt_MinArea
            // 
            resources.ApplyResources(this.txt_MinArea, "txt_MinArea");
            this.txt_MinArea.BackColor = System.Drawing.Color.White;
            this.txt_MinArea.DecimalPlaces = 0;
            this.txt_MinArea.DecMaxValue = new decimal(new int[] {
            99999999,
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
            // srmLabel4
            // 
            resources.ApplyResources(this.srmLabel4, "srmLabel4");
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_PHBlackArea
            // 
            resources.ApplyResources(this.lbl_PHBlackArea, "lbl_PHBlackArea");
            this.lbl_PHBlackArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_PHBlackArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_PHBlackArea.Name = "lbl_PHBlackArea";
            // 
            // srmLabel17
            // 
            resources.ApplyResources(this.srmLabel17, "srmLabel17");
            this.srmLabel17.Name = "srmLabel17";
            this.srmLabel17.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MinBlackArea
            // 
            resources.ApplyResources(this.txt_MinBlackArea, "txt_MinBlackArea");
            this.txt_MinBlackArea.BackColor = System.Drawing.Color.White;
            this.txt_MinBlackArea.DecimalPlaces = 0;
            this.txt_MinBlackArea.DecMaxValue = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txt_MinBlackArea.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_MinBlackArea.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_MinBlackArea.ForeColor = System.Drawing.Color.Black;
            this.txt_MinBlackArea.InputType = SRMControl.InputType.Number;
            this.txt_MinBlackArea.Name = "txt_MinBlackArea";
            this.txt_MinBlackArea.NormalBackColor = System.Drawing.Color.White;
            // 
            // srmLabel16
            // 
            resources.ApplyResources(this.srmLabel16, "srmLabel16");
            this.srmLabel16.Name = "srmLabel16";
            this.srmLabel16.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_PHThreshold
            // 
            resources.ApplyResources(this.btn_PHThreshold, "btn_PHThreshold");
            this.btn_PHThreshold.Name = "btn_PHThreshold";
            this.btn_PHThreshold.UseVisualStyleBackColor = true;
            this.btn_PHThreshold.Click += new System.EventHandler(this.btn_PHThreshold_Click);
            // 
            // lbl_PHThreshold
            // 
            resources.ApplyResources(this.lbl_PHThreshold, "lbl_PHThreshold");
            this.lbl_PHThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_PHThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_PHThreshold.Name = "lbl_PHThreshold";
            // 
            // srmLabel26
            // 
            resources.ApplyResources(this.srmLabel26, "srmLabel26");
            this.srmLabel26.Name = "srmLabel26";
            this.srmLabel26.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_SavePH
            // 
            resources.ApplyResources(this.btn_SavePH, "btn_SavePH");
            this.btn_SavePH.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_SavePH.Name = "btn_SavePH";
            this.btn_SavePH.UseVisualStyleBackColor = true;
            this.btn_SavePH.Click += new System.EventHandler(this.btn_SavePH_Click);
            // 
            // pictureBox10
            // 
            resources.ApplyResources(this.pictureBox10, "pictureBox10");
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.TabStop = false;
            // 
            // srmLabel15
            // 
            resources.ApplyResources(this.srmLabel15, "srmLabel15");
            this.srmLabel15.Name = "srmLabel15";
            this.srmLabel15.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStepOrientROI
            // 
            resources.ApplyResources(this.lbl_TitleStepOrientROI, "lbl_TitleStepOrientROI");
            this.lbl_TitleStepOrientROI.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStepOrientROI.Name = "lbl_TitleStepOrientROI";
            this.lbl_TitleStepOrientROI.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStepDontCareArea
            // 
            resources.ApplyResources(this.lbl_TitleStepDontCareArea, "lbl_TitleStepDontCareArea");
            this.lbl_TitleStepDontCareArea.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStepDontCareArea.Name = "lbl_TitleStepDontCareArea";
            this.lbl_TitleStepDontCareArea.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStepPocket
            // 
            resources.ApplyResources(this.lbl_TitleStepPocket, "lbl_TitleStepPocket");
            this.lbl_TitleStepPocket.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStepPocket.Name = "lbl_TitleStepPocket";
            this.lbl_TitleStepPocket.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.ForeColor = System.Drawing.Color.Black;
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_StepNo
            // 
            resources.ApplyResources(this.lbl_StepNo, "lbl_StepNo");
            this.lbl_StepNo.ForeColor = System.Drawing.Color.Black;
            this.lbl_StepNo.Name = "lbl_StepNo";
            this.lbl_StepNo.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_TitleStep1
            // 
            resources.ApplyResources(this.lbl_TitleStep1, "lbl_TitleStep1");
            this.lbl_TitleStep1.ForeColor = System.Drawing.Color.Black;
            this.lbl_TitleStep1.Name = "lbl_TitleStep1";
            this.lbl_TitleStep1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // LearnPHForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.tabCtrl_Setup);
            this.Controls.Add(this.lbl_TitleStepOrientROI);
            this.Controls.Add(this.lbl_TitleStepDontCareArea);
            this.Controls.Add(this.lbl_TitleStepPocket);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.lbl_StepNo);
            this.Controls.Add(this.lbl_TitleStep1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LearnPHForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LearnPHForm_FormClosing);
            this.Load += new System.EventHandler(this.LearnPHForm_Load);
            this.tabCtrl_Setup.ResumeLayout(false);
            this.tp_Step1.ResumeLayout(false);
            this.srmGroupBox2.ResumeLayout(false);
            this.srmGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMTabControl tabCtrl_Setup;
        private System.Windows.Forms.TabPage tp_Step1;
        private SRMControl.SRMGroupBox srmGroupBox2;
        private System.Windows.Forms.Label lbl_PHBlackArea;
        private SRMControl.SRMLabel srmLabel17;
        private SRMControl.SRMInputBox txt_MinBlackArea;
        private SRMControl.SRMLabel srmLabel16;
        private SRMControl.SRMButton btn_PHThreshold;
        private System.Windows.Forms.Label lbl_PHThreshold;
        private SRMControl.SRMLabel srmLabel26;
        private SRMControl.SRMButton btn_SavePH;
        private System.Windows.Forms.PictureBox pictureBox10;
        private SRMControl.SRMLabel srmLabel15;
        private SRMControl.SRMLabel lbl_TitleStepOrientROI;
        private SRMControl.SRMLabel lbl_TitleStepDontCareArea;
        private SRMControl.SRMLabel lbl_TitleStepPocket;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel lbl_StepNo;
        private SRMControl.SRMLabel lbl_TitleStep1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage tp_Step2;
        private SRMControl.SRMButton btn_Next;
        private SRMControl.SRMButton btn_Previous;
        private SRMControl.SRMInputBox txt_MinArea;
        private SRMControl.SRMLabel srmLabel4;
    }
}