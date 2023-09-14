namespace VisionProcessForm
{
    partial class LearnLeadPocketDontCareAreaFixForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearnLeadPocketDontCareAreaFixForm));
            this.tabCtrl_Lead = new SRMControl.SRMTabControl();
            this.tp_PocketDontCare = new System.Windows.Forms.TabPage();
            this.btn_Save = new SRMControl.SRMButton();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.picDontCare2 = new System.Windows.Forms.PictureBox();
            this.srmLabel8 = new SRMControl.SRMLabel();
            this.gb_ROIDirection = new SRMControl.SRMGroupBox();
            this.chk_LeftROI = new SRMControl.SRMCheckBox();
            this.chk_RightROI = new SRMControl.SRMCheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.chk_TopROI = new SRMControl.SRMCheckBox();
            this.chk_BottomROI = new SRMControl.SRMCheckBox();
            this.srmLabel7 = new SRMControl.SRMLabel();
            this.btn_Next = new SRMControl.SRMButton();
            this.btn_Previous = new SRMControl.SRMButton();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.lbl_Step1 = new SRMControl.SRMLabel();
            this.lbl_StepNo = new SRMControl.SRMLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabCtrl_Lead.SuspendLayout();
            this.tp_PocketDontCare.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDontCare2)).BeginInit();
            this.gb_ROIDirection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // tabCtrl_Lead
            // 
            this.tabCtrl_Lead.Controls.Add(this.tp_PocketDontCare);
            resources.ApplyResources(this.tabCtrl_Lead, "tabCtrl_Lead");
            this.tabCtrl_Lead.Name = "tabCtrl_Lead";
            this.tabCtrl_Lead.SelectedIndex = 0;
            this.tabCtrl_Lead.TabStop = false;
            // 
            // tp_PocketDontCare
            // 
            this.tp_PocketDontCare.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.tp_PocketDontCare.Controls.Add(this.btn_Save);
            this.tp_PocketDontCare.Controls.Add(this.pictureBox11);
            this.tp_PocketDontCare.Controls.Add(this.picDontCare2);
            this.tp_PocketDontCare.Controls.Add(this.srmLabel8);
            this.tp_PocketDontCare.Controls.Add(this.gb_ROIDirection);
            this.tp_PocketDontCare.Controls.Add(this.srmLabel7);
            resources.ApplyResources(this.tp_PocketDontCare, "tp_PocketDontCare");
            this.tp_PocketDontCare.Name = "tp_PocketDontCare";
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // pictureBox11
            // 
            resources.ApplyResources(this.pictureBox11, "pictureBox11");
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.TabStop = false;
            // 
            // picDontCare2
            // 
            this.picDontCare2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.picDontCare2, "picDontCare2");
            this.picDontCare2.Name = "picDontCare2";
            this.picDontCare2.TabStop = false;
            // 
            // srmLabel8
            // 
            resources.ApplyResources(this.srmLabel8, "srmLabel8");
            this.srmLabel8.Name = "srmLabel8";
            this.srmLabel8.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // gb_ROIDirection
            // 
            this.gb_ROIDirection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.gb_ROIDirection.Controls.Add(this.chk_LeftROI);
            this.gb_ROIDirection.Controls.Add(this.chk_RightROI);
            this.gb_ROIDirection.Controls.Add(this.pictureBox2);
            this.gb_ROIDirection.Controls.Add(this.chk_TopROI);
            this.gb_ROIDirection.Controls.Add(this.chk_BottomROI);
            resources.ApplyResources(this.gb_ROIDirection, "gb_ROIDirection");
            this.gb_ROIDirection.Name = "gb_ROIDirection";
            this.gb_ROIDirection.TabStop = false;
            // 
            // chk_LeftROI
            // 
            resources.ApplyResources(this.chk_LeftROI, "chk_LeftROI");
            this.chk_LeftROI.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_LeftROI.Name = "chk_LeftROI";
            this.chk_LeftROI.Selected = true;
            this.chk_LeftROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_LeftROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_LeftROI.UseVisualStyleBackColor = true;
            this.chk_LeftROI.Click += new System.EventHandler(this.chk_ROI_Click);
            // 
            // chk_RightROI
            // 
            this.chk_RightROI.CheckedColor = System.Drawing.Color.GreenYellow;
            resources.ApplyResources(this.chk_RightROI, "chk_RightROI");
            this.chk_RightROI.Name = "chk_RightROI";
            this.chk_RightROI.Selected = true;
            this.chk_RightROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_RightROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_RightROI.UseVisualStyleBackColor = true;
            this.chk_RightROI.Click += new System.EventHandler(this.chk_ROI_Click);
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // chk_TopROI
            // 
            resources.ApplyResources(this.chk_TopROI, "chk_TopROI");
            this.chk_TopROI.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_TopROI.Name = "chk_TopROI";
            this.chk_TopROI.Selected = true;
            this.chk_TopROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_TopROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_TopROI.UseVisualStyleBackColor = true;
            this.chk_TopROI.Click += new System.EventHandler(this.chk_ROI_Click);
            // 
            // chk_BottomROI
            // 
            resources.ApplyResources(this.chk_BottomROI, "chk_BottomROI");
            this.chk_BottomROI.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_BottomROI.Name = "chk_BottomROI";
            this.chk_BottomROI.Selected = true;
            this.chk_BottomROI.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_BottomROI.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_BottomROI.UseVisualStyleBackColor = true;
            this.chk_BottomROI.Click += new System.EventHandler(this.chk_ROI_Click);
            // 
            // srmLabel7
            // 
            resources.ApplyResources(this.srmLabel7, "srmLabel7");
            this.srmLabel7.Name = "srmLabel7";
            this.srmLabel7.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Next
            // 
            resources.ApplyResources(this.btn_Next, "btn_Next");
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.UseVisualStyleBackColor = true;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // btn_Previous
            // 
            resources.ApplyResources(this.btn_Previous, "btn_Previous");
            this.btn_Previous.Name = "btn_Previous";
            this.btn_Previous.UseVisualStyleBackColor = true;
            this.btn_Previous.Click += new System.EventHandler(this.btn_Previous_Click);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // lbl_Step1
            // 
            resources.ApplyResources(this.lbl_Step1, "lbl_Step1");
            this.lbl_Step1.ForeColor = System.Drawing.Color.Black;
            this.lbl_Step1.Name = "lbl_Step1";
            this.lbl_Step1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_StepNo
            // 
            resources.ApplyResources(this.lbl_StepNo, "lbl_StepNo");
            this.lbl_StepNo.ForeColor = System.Drawing.Color.Black;
            this.lbl_StepNo.Name = "lbl_StepNo";
            this.lbl_StepNo.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // LearnLeadPocketDontCareAreaFixForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_Next);
            this.Controls.Add(this.btn_Previous);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.tabCtrl_Lead);
            this.Controls.Add(this.lbl_Step1);
            this.Controls.Add(this.lbl_StepNo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LearnLeadPocketDontCareAreaFixForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LearnLeadPocketDontCareAreaFixForm_FormClosing);
            this.Load += new System.EventHandler(this.LearnLeadPocketDontCareAreaFixForm_Load);
            this.tabCtrl_Lead.ResumeLayout(false);
            this.tp_PocketDontCare.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDontCare2)).EndInit();
            this.gb_ROIDirection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMTabControl tabCtrl_Lead;
        private System.Windows.Forms.TabPage tp_PocketDontCare;
        private SRMControl.SRMButton btn_Save;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.PictureBox picDontCare2;
        private SRMControl.SRMLabel srmLabel8;
        private SRMControl.SRMGroupBox gb_ROIDirection;
        private SRMControl.SRMCheckBox chk_LeftROI;
        private SRMControl.SRMCheckBox chk_RightROI;
        private System.Windows.Forms.PictureBox pictureBox2;
        private SRMControl.SRMCheckBox chk_TopROI;
        private SRMControl.SRMCheckBox chk_BottomROI;
        private SRMControl.SRMLabel srmLabel7;
        private SRMControl.SRMButton btn_Next;
        private SRMControl.SRMButton btn_Previous;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMLabel lbl_Step1;
        private SRMControl.SRMLabel lbl_StepNo;
        private System.Windows.Forms.Timer timer1;
    }
}