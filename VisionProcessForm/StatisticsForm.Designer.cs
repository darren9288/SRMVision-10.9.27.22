namespace VisionProcessForm
{
    partial class StatisticsForm
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
            this.chk_TestUnit = new SRMControl.SRMCheckBox();
            this.chk_RetestUnit = new SRMControl.SRMCheckBox();
            this.btn_Images = new SRMControl.SRMButton();
            this.btn_Close = new SRMControl.SRMButton();
            this.btn_AddStatistics = new SRMControl.SRMButton();
            this.lbl_TestUnitCount = new SRMControl.SRMLabel();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.srmLabel4 = new SRMControl.SRMLabel();
            this.lbl_RetestUnitCount = new SRMControl.SRMLabel();
            this.btn_Clear = new SRMControl.SRMButton();
            this.group_Threshold = new SRMControl.SRMGroupBox();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.cbo_TemplateNo = new SRMControl.SRMComboBox();
            this.srmGroupBox1 = new SRMControl.SRMGroupBox();
            this.group_Threshold.SuspendLayout();
            this.srmGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chk_TestUnit
            // 
            this.chk_TestUnit.Checked = true;
            this.chk_TestUnit.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_TestUnit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_TestUnit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_TestUnit.Location = new System.Drawing.Point(6, 19);
            this.chk_TestUnit.Name = "chk_TestUnit";
            this.chk_TestUnit.Selected = false;
            this.chk_TestUnit.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_TestUnit.Size = new System.Drawing.Size(93, 24);
            this.chk_TestUnit.TabIndex = 58;
            this.chk_TestUnit.Text = "Test Unit";
            this.chk_TestUnit.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_TestUnit.UseVisualStyleBackColor = true;
            this.chk_TestUnit.CheckedChanged += new System.EventHandler(this.chk_TestUnit_CheckedChanged);
            // 
            // chk_RetestUnit
            // 
            this.chk_RetestUnit.Checked = true;
            this.chk_RetestUnit.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_RetestUnit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_RetestUnit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chk_RetestUnit.Location = new System.Drawing.Point(6, 49);
            this.chk_RetestUnit.Name = "chk_RetestUnit";
            this.chk_RetestUnit.Selected = false;
            this.chk_RetestUnit.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_RetestUnit.Size = new System.Drawing.Size(93, 24);
            this.chk_RetestUnit.TabIndex = 59;
            this.chk_RetestUnit.Text = "Retest Unit";
            this.chk_RetestUnit.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_RetestUnit.UseVisualStyleBackColor = true;
            this.chk_RetestUnit.CheckedChanged += new System.EventHandler(this.chk_RetestUnit_CheckedChanged);
            // 
            // btn_Images
            // 
            this.btn_Images.Location = new System.Drawing.Point(9, 78);
            this.btn_Images.Name = "btn_Images";
            this.btn_Images.Size = new System.Drawing.Size(107, 34);
            this.btn_Images.TabIndex = 60;
            this.btn_Images.Text = "Display Images";
            this.btn_Images.UseVisualStyleBackColor = true;
            this.btn_Images.Click += new System.EventHandler(this.btn_Images_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(157, 264);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(80, 34);
            this.btn_Close.TabIndex = 62;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_AddStatistics
            // 
            this.btn_AddStatistics.Location = new System.Drawing.Point(157, 184);
            this.btn_AddStatistics.Name = "btn_AddStatistics";
            this.btn_AddStatistics.Size = new System.Drawing.Size(80, 34);
            this.btn_AddStatistics.TabIndex = 63;
            this.btn_AddStatistics.Text = "Add";
            this.btn_AddStatistics.UseVisualStyleBackColor = true;
            this.btn_AddStatistics.Click += new System.EventHandler(this.btn_AddStatistics_Click);
            // 
            // lbl_TestUnitCount
            // 
            this.lbl_TestUnitCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_TestUnitCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_TestUnitCount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_TestUnitCount.Location = new System.Drawing.Point(91, 16);
            this.lbl_TestUnitCount.Name = "lbl_TestUnitCount";
            this.lbl_TestUnitCount.Size = new System.Drawing.Size(66, 20);
            this.lbl_TestUnitCount.TabIndex = 65;
            this.lbl_TestUnitCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_TestUnitCount.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel3
            // 
            this.srmLabel3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel3.Location = new System.Drawing.Point(6, 16);
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.Size = new System.Drawing.Size(69, 20);
            this.srmLabel3.TabIndex = 66;
            this.srmLabel3.Text = "Test Unit : ";
            this.srmLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel4
            // 
            this.srmLabel4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.srmLabel4.Location = new System.Drawing.Point(6, 46);
            this.srmLabel4.Name = "srmLabel4";
            this.srmLabel4.Size = new System.Drawing.Size(69, 20);
            this.srmLabel4.TabIndex = 67;
            this.srmLabel4.Text = "Retest Unit : ";
            this.srmLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.srmLabel4.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_RetestUnitCount
            // 
            this.lbl_RetestUnitCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_RetestUnitCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_RetestUnitCount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_RetestUnitCount.Location = new System.Drawing.Point(91, 46);
            this.lbl_RetestUnitCount.Name = "lbl_RetestUnitCount";
            this.lbl_RetestUnitCount.Size = new System.Drawing.Size(66, 20);
            this.lbl_RetestUnitCount.TabIndex = 68;
            this.lbl_RetestUnitCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_RetestUnitCount.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Clear
            // 
            this.btn_Clear.Location = new System.Drawing.Point(157, 224);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(80, 34);
            this.btn_Clear.TabIndex = 69;
            this.btn_Clear.Text = "Clear";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // group_Threshold
            // 
            this.group_Threshold.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_Threshold.Controls.Add(this.chk_TestUnit);
            this.group_Threshold.Controls.Add(this.chk_RetestUnit);
            this.group_Threshold.Location = new System.Drawing.Point(15, 180);
            this.group_Threshold.Name = "group_Threshold";
            this.group_Threshold.Size = new System.Drawing.Size(136, 78);
            this.group_Threshold.TabIndex = 70;
            this.group_Threshold.TabStop = false;
            this.group_Threshold.Text = "Please select unit";
            // 
            // srmLabel2
            // 
            this.srmLabel2.AutoSize = true;
            this.srmLabel2.Location = new System.Drawing.Point(12, 155);
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.Size = new System.Drawing.Size(101, 13);
            this.srmLabel2.TabIndex = 126;
            this.srmLabel2.Text = "Select Template No";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // cbo_TemplateNo
            // 
            this.cbo_TemplateNo.BackColor = System.Drawing.Color.White;
            this.cbo_TemplateNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_TemplateNo.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbo_TemplateNo.FormattingEnabled = true;
            this.cbo_TemplateNo.ItemHeight = 20;
            this.cbo_TemplateNo.Location = new System.Drawing.Point(119, 148);
            this.cbo_TemplateNo.Name = "cbo_TemplateNo";
            this.cbo_TemplateNo.NormalBackColor = System.Drawing.Color.White;
            this.cbo_TemplateNo.Size = new System.Drawing.Size(113, 26);
            this.cbo_TemplateNo.TabIndex = 127;
            this.cbo_TemplateNo.SelectedIndexChanged += new System.EventHandler(this.cbo_TemplateNo_SelectedIndexChanged);
            // 
            // srmGroupBox1
            // 
            this.srmGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox1.Controls.Add(this.srmLabel3);
            this.srmGroupBox1.Controls.Add(this.lbl_TestUnitCount);
            this.srmGroupBox1.Controls.Add(this.srmLabel4);
            this.srmGroupBox1.Controls.Add(this.lbl_RetestUnitCount);
            this.srmGroupBox1.Controls.Add(this.btn_Images);
            this.srmGroupBox1.Location = new System.Drawing.Point(15, 12);
            this.srmGroupBox1.Name = "srmGroupBox1";
            this.srmGroupBox1.Size = new System.Drawing.Size(217, 124);
            this.srmGroupBox1.TabIndex = 128;
            this.srmGroupBox1.TabStop = false;
            this.srmGroupBox1.Text = "Current Statistics Images No. :";
            // 
            // StatisticsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(244, 310);
            this.ControlBox = false;
            this.Controls.Add(this.srmGroupBox1);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.cbo_TemplateNo);
            this.Controls.Add(this.group_Threshold);
            this.Controls.Add(this.btn_Clear);
            this.Controls.Add(this.btn_AddStatistics);
            this.Controls.Add(this.btn_Close);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StatisticsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Statistics Form";
            this.group_Threshold.ResumeLayout(false);
            this.srmGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMCheckBox chk_TestUnit;
        private SRMControl.SRMCheckBox chk_RetestUnit;
        private SRMControl.SRMButton btn_Images;
        private SRMControl.SRMButton btn_Close;
        private SRMControl.SRMButton btn_AddStatistics;
        private SRMControl.SRMLabel lbl_TestUnitCount;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMLabel srmLabel4;
        private SRMControl.SRMLabel lbl_RetestUnitCount;
        private SRMControl.SRMButton btn_Clear;
        private SRMControl.SRMGroupBox group_Threshold;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMComboBox cbo_TemplateNo;
        private SRMControl.SRMGroupBox srmGroupBox1;
    }
}