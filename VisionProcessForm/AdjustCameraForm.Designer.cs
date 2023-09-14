namespace VisionProcessForm
{
    partial class AdjustCameraForm
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
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.group_LSHSetting = new SRMControl.SRMGroupBox();
            this.btn_OnePush = new SRMControl.SRMButton();
            this.chk_Auto = new SRMControl.SRMCheckBox();
            this.lbl_Saturation = new SRMControl.SRMLabel();
            this.txt_VR = new SRMControl.SRMInputBox();
            this.trackBar_VR = new System.Windows.Forms.TrackBar();
            this.lbl_Hue = new SRMControl.SRMLabel();
            this.trackBar_UB = new System.Windows.Forms.TrackBar();
            this.txt_UB = new SRMControl.SRMInputBox();
            this.srmGroupBox1 = new SRMControl.SRMGroupBox();
            this.chk_GammaOnOff = new SRMControl.SRMCheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.group_LSHSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_VR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_UB)).BeginInit();
            this.srmGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Cancel.Location = new System.Drawing.Point(218, 275);
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(81, 31);
            this.btn_Cancel.TabIndex = 32;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Save.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(134, 275);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(81, 31);
            this.btn_Save.TabIndex = 31;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // group_LSHSetting
            // 
            this.group_LSHSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.group_LSHSetting.Controls.Add(this.btn_OnePush);
            this.group_LSHSetting.Controls.Add(this.chk_Auto);
            this.group_LSHSetting.Controls.Add(this.lbl_Saturation);
            this.group_LSHSetting.Controls.Add(this.txt_VR);
            this.group_LSHSetting.Controls.Add(this.trackBar_VR);
            this.group_LSHSetting.Controls.Add(this.lbl_Hue);
            this.group_LSHSetting.Controls.Add(this.trackBar_UB);
            this.group_LSHSetting.Controls.Add(this.txt_UB);
            this.group_LSHSetting.Location = new System.Drawing.Point(12, 10);
            this.group_LSHSetting.Name = "group_LSHSetting";
            this.group_LSHSetting.Size = new System.Drawing.Size(287, 185);
            this.group_LSHSetting.TabIndex = 62;
            this.group_LSHSetting.TabStop = false;
            this.group_LSHSetting.Text = "White Balance";
            // 
            // btn_OnePush
            // 
            this.btn_OnePush.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.btn_OnePush.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_OnePush.Location = new System.Drawing.Point(109, 137);
            this.btn_OnePush.Margin = new System.Windows.Forms.Padding(4);
            this.btn_OnePush.Name = "btn_OnePush";
            this.btn_OnePush.Size = new System.Drawing.Size(94, 42);
            this.btn_OnePush.TabIndex = 74;
            this.btn_OnePush.Text = "One Push";
            this.btn_OnePush.UseVisualStyleBackColor = true;
            this.btn_OnePush.Click += new System.EventHandler(this.btn_OnePush_Click);
            // 
            // chk_Auto
            // 
            this.chk_Auto.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_Auto.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Auto.Location = new System.Drawing.Point(8, 137);
            this.chk_Auto.Name = "chk_Auto";
            this.chk_Auto.Selected = false;
            this.chk_Auto.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Auto.Size = new System.Drawing.Size(94, 42);
            this.chk_Auto.TabIndex = 63;
            this.chk_Auto.Text = "Auto";
            this.chk_Auto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chk_Auto.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Auto.UseVisualStyleBackColor = true;
            this.chk_Auto.Click += new System.EventHandler(this.chk_Auto_Click);
            // 
            // lbl_Saturation
            // 
            this.lbl_Saturation.AutoSize = true;
            this.lbl_Saturation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Saturation.Location = new System.Drawing.Point(17, 91);
            this.lbl_Saturation.Name = "lbl_Saturation";
            this.lbl_Saturation.Size = new System.Drawing.Size(27, 13);
            this.lbl_Saturation.TabIndex = 67;
            this.lbl_Saturation.Text = "V/R";
            this.lbl_Saturation.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_VR
            // 
            this.txt_VR.BackColor = System.Drawing.Color.White;
            this.txt_VR.DecimalPlaces = 0;
            this.txt_VR.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_VR.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_VR.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_VR.ForeColor = System.Drawing.Color.Black;
            this.txt_VR.InputType = SRMControl.InputType.Number;
            this.txt_VR.Location = new System.Drawing.Point(75, 87);
            this.txt_VR.Name = "txt_VR";
            this.txt_VR.NormalBackColor = System.Drawing.Color.White;
            this.txt_VR.Size = new System.Drawing.Size(55, 20);
            this.txt_VR.TabIndex = 68;
            this.txt_VR.Text = "125";
            this.txt_VR.TextChanged += new System.EventHandler(this.txt_VR_TextChanged);
            // 
            // trackBar_VR
            // 
            this.trackBar_VR.AutoSize = false;
            this.trackBar_VR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_VR.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.trackBar_VR.LargeChange = 1;
            this.trackBar_VR.Location = new System.Drawing.Point(4, 108);
            this.trackBar_VR.Maximum = 1022;
            this.trackBar_VR.Name = "trackBar_VR";
            this.trackBar_VR.Size = new System.Drawing.Size(282, 23);
            this.trackBar_VR.TabIndex = 73;
            this.trackBar_VR.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_VR.Value = 125;
            this.trackBar_VR.Scroll += new System.EventHandler(this.trackBar_VR_Scroll);
            // 
            // lbl_Hue
            // 
            this.lbl_Hue.AutoSize = true;
            this.lbl_Hue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Hue.Location = new System.Drawing.Point(17, 35);
            this.lbl_Hue.Name = "lbl_Hue";
            this.lbl_Hue.Size = new System.Drawing.Size(27, 13);
            this.lbl_Hue.TabIndex = 70;
            this.lbl_Hue.Text = "U/B";
            this.lbl_Hue.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // trackBar_UB
            // 
            this.trackBar_UB.AutoSize = false;
            this.trackBar_UB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_UB.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.trackBar_UB.LargeChange = 1;
            this.trackBar_UB.Location = new System.Drawing.Point(4, 53);
            this.trackBar_UB.Maximum = 1022;
            this.trackBar_UB.Name = "trackBar_UB";
            this.trackBar_UB.Size = new System.Drawing.Size(282, 23);
            this.trackBar_UB.TabIndex = 69;
            this.trackBar_UB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_UB.Value = 125;
            this.trackBar_UB.Scroll += new System.EventHandler(this.trackBar_UB_Scroll);
            // 
            // txt_UB
            // 
            this.txt_UB.BackColor = System.Drawing.Color.White;
            this.txt_UB.DecimalPlaces = 0;
            this.txt_UB.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_UB.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_UB.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_UB.ForeColor = System.Drawing.Color.Black;
            this.txt_UB.InputType = SRMControl.InputType.Number;
            this.txt_UB.Location = new System.Drawing.Point(75, 32);
            this.txt_UB.Name = "txt_UB";
            this.txt_UB.NormalBackColor = System.Drawing.Color.White;
            this.txt_UB.Size = new System.Drawing.Size(55, 20);
            this.txt_UB.TabIndex = 71;
            this.txt_UB.Text = "125";
            this.txt_UB.TextChanged += new System.EventHandler(this.txt_UB_TextChanged);
            // 
            // srmGroupBox1
            // 
            this.srmGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(150)))), ((int)(((byte)(185)))));
            this.srmGroupBox1.Controls.Add(this.chk_GammaOnOff);
            this.srmGroupBox1.Location = new System.Drawing.Point(12, 201);
            this.srmGroupBox1.Name = "srmGroupBox1";
            this.srmGroupBox1.Size = new System.Drawing.Size(287, 67);
            this.srmGroupBox1.TabIndex = 63;
            this.srmGroupBox1.TabStop = false;
            this.srmGroupBox1.Text = "Gamma";
            // 
            // chk_GammaOnOff
            // 
            this.chk_GammaOnOff.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_GammaOnOff.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_GammaOnOff.Location = new System.Drawing.Point(8, 19);
            this.chk_GammaOnOff.Name = "chk_GammaOnOff";
            this.chk_GammaOnOff.Selected = false;
            this.chk_GammaOnOff.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_GammaOnOff.Size = new System.Drawing.Size(94, 42);
            this.chk_GammaOnOff.TabIndex = 64;
            this.chk_GammaOnOff.Text = "On";
            this.chk_GammaOnOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chk_GammaOnOff.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_GammaOnOff.UseVisualStyleBackColor = true;
            this.chk_GammaOnOff.Click += new System.EventHandler(this.chk_GammaOnOff_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // AdjustCameraForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(310, 355);
            this.ControlBox = false;
            this.Controls.Add(this.srmGroupBox1);
            this.Controls.Add(this.group_LSHSetting);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdjustCameraForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Adjust Camera Form";
            this.group_LSHSetting.ResumeLayout(false);
            this.group_LSHSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_VR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_UB)).EndInit();
            this.srmGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_Save;
        private SRMControl.SRMGroupBox group_LSHSetting;
        private SRMControl.SRMLabel lbl_Saturation;
        private SRMControl.SRMInputBox txt_VR;
        private System.Windows.Forms.TrackBar trackBar_VR;
        private SRMControl.SRMLabel lbl_Hue;
        private System.Windows.Forms.TrackBar trackBar_UB;
        private SRMControl.SRMInputBox txt_UB;
        private SRMControl.SRMCheckBox chk_Auto;
        private SRMControl.SRMGroupBox srmGroupBox1;
        private SRMControl.SRMCheckBox chk_GammaOnOff;
        private System.Windows.Forms.Timer timer1;
        private SRMControl.SRMButton btn_OnePush;
    }
}