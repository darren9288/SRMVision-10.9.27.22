namespace VisionProcessForm
{
    partial class CheckPresentOtherSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckPresentOtherSettingForm));
            this.lbl_Title = new SRMControl.SRMLabel();
            this.btn_ThresholdUnitROI = new SRMControl.SRMButton();
            this.srmGroupBox4 = new SRMControl.SRMGroupBox();
            this.lbl_UnitPresentThreshold = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Close = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.srmGroupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_Title
            // 
            resources.ApplyResources(this.lbl_Title, "lbl_Title");
            this.lbl_Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_Title.ForeColor = System.Drawing.Color.Black;
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_ThresholdUnitROI
            // 
            resources.ApplyResources(this.btn_ThresholdUnitROI, "btn_ThresholdUnitROI");
            this.btn_ThresholdUnitROI.Name = "btn_ThresholdUnitROI";
            this.btn_ThresholdUnitROI.UseVisualStyleBackColor = true;
            this.btn_ThresholdUnitROI.Click += new System.EventHandler(this.btn_ThresholdUnitROI_Click);
            // 
            // srmGroupBox4
            // 
            resources.ApplyResources(this.srmGroupBox4, "srmGroupBox4");
            this.srmGroupBox4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox4.Controls.Add(this.lbl_UnitPresentThreshold);
            this.srmGroupBox4.Controls.Add(this.label1);
            this.srmGroupBox4.Controls.Add(this.btn_ThresholdUnitROI);
            this.srmGroupBox4.Name = "srmGroupBox4";
            this.srmGroupBox4.TabStop = false;
            // 
            // lbl_UnitPresentThreshold
            // 
            resources.ApplyResources(this.lbl_UnitPresentThreshold, "lbl_UnitPresentThreshold");
            this.lbl_UnitPresentThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.lbl_UnitPresentThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_UnitPresentThreshold.Name = "lbl_UnitPresentThreshold";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            // CheckPresentOtherSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.srmGroupBox4);
            this.Controls.Add(this.lbl_Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CheckPresentOtherSettingForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CheckPresentOtherSettingForm_FormClosing);
            this.Load += new System.EventHandler(this.CheckPresentOtherSettingForm_Load);
            this.srmGroupBox4.ResumeLayout(false);
            this.srmGroupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMLabel lbl_Title;
        private SRMControl.SRMButton btn_ThresholdUnitROI;
        private SRMControl.SRMGroupBox srmGroupBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_UnitPresentThreshold;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMButton btn_Save;
    }
}