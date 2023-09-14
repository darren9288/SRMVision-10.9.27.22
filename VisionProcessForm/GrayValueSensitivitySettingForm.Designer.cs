namespace VisionProcessForm
{
    partial class GrayValueSensitivitySettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GrayValueSensitivitySettingForm));
            this.grp_GrayValueThreshold = new System.Windows.Forms.GroupBox();
            this.lbl_AverageGrayValue = new SRMControl.SRMLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chk_ViewThresholdImage = new SRMControl.SRMCheckBox();
            this.srmLabel54 = new SRMControl.SRMLabel();
            this.txt_DarkSensitivity = new System.Windows.Forms.NumericUpDown();
            this.srmLabel53 = new SRMControl.SRMLabel();
            this.txt_BrightSensitivity = new System.Windows.Forms.NumericUpDown();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.srmLabel51 = new SRMControl.SRMLabel();
            this.txt_MergeSensitivity = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.srmLabel50 = new SRMControl.SRMLabel();
            this.txt_InspectionAreaGrayValueSensitivity = new System.Windows.Forms.NumericUpDown();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pic_ROI = new System.Windows.Forms.PictureBox();
            this.cbo_ImageBrightOrDark = new SRMControl.SRMComboBox();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chk_WantUsePackageSetting = new SRMControl.SRMCheckBox();
            this.grp_GrayValueThreshold.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_DarkSensitivity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_BrightSensitivity)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_MergeSensitivity)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_InspectionAreaGrayValueSensitivity)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_ROI)).BeginInit();
            this.SuspendLayout();
            // 
            // grp_GrayValueThreshold
            // 
            resources.ApplyResources(this.grp_GrayValueThreshold, "grp_GrayValueThreshold");
            this.grp_GrayValueThreshold.Controls.Add(this.lbl_AverageGrayValue);
            this.grp_GrayValueThreshold.Controls.Add(this.groupBox3);
            this.grp_GrayValueThreshold.Controls.Add(this.srmLabel2);
            this.grp_GrayValueThreshold.Controls.Add(this.groupBox2);
            this.grp_GrayValueThreshold.Controls.Add(this.groupBox1);
            this.grp_GrayValueThreshold.Name = "grp_GrayValueThreshold";
            this.grp_GrayValueThreshold.TabStop = false;
            // 
            // lbl_AverageGrayValue
            // 
            resources.ApplyResources(this.lbl_AverageGrayValue, "lbl_AverageGrayValue");
            this.lbl_AverageGrayValue.Name = "lbl_AverageGrayValue";
            this.lbl_AverageGrayValue.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.chk_ViewThresholdImage);
            this.groupBox3.Controls.Add(this.srmLabel54);
            this.groupBox3.Controls.Add(this.txt_DarkSensitivity);
            this.groupBox3.Controls.Add(this.srmLabel53);
            this.groupBox3.Controls.Add(this.txt_BrightSensitivity);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // chk_ViewThresholdImage
            // 
            resources.ApplyResources(this.chk_ViewThresholdImage, "chk_ViewThresholdImage");
            this.chk_ViewThresholdImage.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_ViewThresholdImage.Name = "chk_ViewThresholdImage";
            this.chk_ViewThresholdImage.Selected = false;
            this.chk_ViewThresholdImage.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_ViewThresholdImage.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_ViewThresholdImage.UseVisualStyleBackColor = true;
            this.chk_ViewThresholdImage.Click += new System.EventHandler(this.chk_ViewThresholdImage_Click);
            // 
            // srmLabel54
            // 
            resources.ApplyResources(this.srmLabel54, "srmLabel54");
            this.srmLabel54.Name = "srmLabel54";
            this.srmLabel54.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_DarkSensitivity
            // 
            resources.ApplyResources(this.txt_DarkSensitivity, "txt_DarkSensitivity");
            this.txt_DarkSensitivity.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_DarkSensitivity.Name = "txt_DarkSensitivity";
            this.txt_DarkSensitivity.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.txt_DarkSensitivity.ValueChanged += new System.EventHandler(this.txt_DarkSensitivity_ValueChanged);
            // 
            // srmLabel53
            // 
            resources.ApplyResources(this.srmLabel53, "srmLabel53");
            this.srmLabel53.Name = "srmLabel53";
            this.srmLabel53.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_BrightSensitivity
            // 
            resources.ApplyResources(this.txt_BrightSensitivity, "txt_BrightSensitivity");
            this.txt_BrightSensitivity.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_BrightSensitivity.Name = "txt_BrightSensitivity";
            this.txt_BrightSensitivity.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.txt_BrightSensitivity.ValueChanged += new System.EventHandler(this.txt_BrightSensitivity_ValueChanged);
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.srmLabel51);
            this.groupBox2.Controls.Add(this.txt_MergeSensitivity);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // srmLabel51
            // 
            resources.ApplyResources(this.srmLabel51, "srmLabel51");
            this.srmLabel51.Name = "srmLabel51";
            this.srmLabel51.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_MergeSensitivity
            // 
            resources.ApplyResources(this.txt_MergeSensitivity, "txt_MergeSensitivity");
            this.txt_MergeSensitivity.Name = "txt_MergeSensitivity";
            this.txt_MergeSensitivity.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.txt_MergeSensitivity.ValueChanged += new System.EventHandler(this.txt_MergeSensitivity_ValueChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.srmLabel50);
            this.groupBox1.Controls.Add(this.txt_InspectionAreaGrayValueSensitivity);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // srmLabel50
            // 
            resources.ApplyResources(this.srmLabel50, "srmLabel50");
            this.srmLabel50.Name = "srmLabel50";
            this.srmLabel50.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_InspectionAreaGrayValueSensitivity
            // 
            resources.ApplyResources(this.txt_InspectionAreaGrayValueSensitivity, "txt_InspectionAreaGrayValueSensitivity");
            this.txt_InspectionAreaGrayValueSensitivity.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_InspectionAreaGrayValueSensitivity.Name = "txt_InspectionAreaGrayValueSensitivity";
            this.txt_InspectionAreaGrayValueSensitivity.Value = new decimal(new int[] {
            45,
            0,
            0,
            0});
            this.txt_InspectionAreaGrayValueSensitivity.ValueChanged += new System.EventHandler(this.txt_InspectionAreaGrayValueSensitivity_ValueChanged);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pic_ROI);
            this.panel1.Name = "panel1";
            // 
            // pic_ROI
            // 
            resources.ApplyResources(this.pic_ROI, "pic_ROI");
            this.pic_ROI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.pic_ROI.Name = "pic_ROI";
            this.pic_ROI.TabStop = false;
            // 
            // cbo_ImageBrightOrDark
            // 
            resources.ApplyResources(this.cbo_ImageBrightOrDark, "cbo_ImageBrightOrDark");
            this.cbo_ImageBrightOrDark.BackColor = System.Drawing.Color.White;
            this.cbo_ImageBrightOrDark.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_ImageBrightOrDark.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_ImageBrightOrDark.FormattingEnabled = true;
            this.cbo_ImageBrightOrDark.Items.AddRange(new object[] {
            resources.GetString("cbo_ImageBrightOrDark.Items"),
            resources.GetString("cbo_ImageBrightOrDark.Items1")});
            this.cbo_ImageBrightOrDark.Name = "cbo_ImageBrightOrDark";
            this.cbo_ImageBrightOrDark.NormalBackColor = System.Drawing.Color.White;
            this.cbo_ImageBrightOrDark.SelectedIndexChanged += new System.EventHandler(this.cbo_ImageBrightOrDark_SelectedIndexChanged);
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // chk_WantUsePackageSetting
            // 
            resources.ApplyResources(this.chk_WantUsePackageSetting, "chk_WantUsePackageSetting");
            this.chk_WantUsePackageSetting.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_WantUsePackageSetting.Name = "chk_WantUsePackageSetting";
            this.chk_WantUsePackageSetting.Selected = false;
            this.chk_WantUsePackageSetting.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_WantUsePackageSetting.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_WantUsePackageSetting.UseVisualStyleBackColor = true;
            this.chk_WantUsePackageSetting.Click += new System.EventHandler(this.chk_WantUsePackageSetting_Click);
            // 
            // GrayValueSensitivitySettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.chk_WantUsePackageSetting);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.cbo_ImageBrightOrDark);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.grp_GrayValueThreshold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GrayValueSensitivitySettingForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.GrayValueSensitivitySettingForm_Load);
            this.grp_GrayValueThreshold.ResumeLayout(false);
            this.grp_GrayValueThreshold.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_DarkSensitivity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_BrightSensitivity)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_MergeSensitivity)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_InspectionAreaGrayValueSensitivity)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_ROI)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grp_GrayValueThreshold;
        private System.Windows.Forms.GroupBox groupBox3;
        private SRMControl.SRMLabel srmLabel54;
        private System.Windows.Forms.NumericUpDown txt_DarkSensitivity;
        private SRMControl.SRMLabel srmLabel53;
        private System.Windows.Forms.NumericUpDown txt_BrightSensitivity;
        private System.Windows.Forms.GroupBox groupBox2;
        private SRMControl.SRMLabel srmLabel51;
        private System.Windows.Forms.NumericUpDown txt_MergeSensitivity;
        private System.Windows.Forms.GroupBox groupBox1;
        private SRMControl.SRMLabel srmLabel50;
        private System.Windows.Forms.NumericUpDown txt_InspectionAreaGrayValueSensitivity;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pic_ROI;
        private SRMControl.SRMComboBox cbo_ImageBrightOrDark;
        private SRMControl.SRMLabel srmLabel1;
        private System.Windows.Forms.Timer timer1;
        private SRMControl.SRMCheckBox chk_ViewThresholdImage;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel lbl_AverageGrayValue;
        private SRMControl.SRMCheckBox chk_WantUsePackageSetting;
    }
}