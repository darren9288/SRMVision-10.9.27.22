namespace VisionProcessForm
{
    partial class RGBThresholdForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RGBThresholdForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.lbl_HueTolerance = new SRMControl.SRMLabel();
            this.lbl_SaturationTolerance = new SRMControl.SRMLabel();
            this.txt_GreenTolerance = new System.Windows.Forms.NumericUpDown();
            this.txt_RedTolerance = new System.Windows.Forms.NumericUpDown();
            this.lbl_LightnessTolerance = new SRMControl.SRMLabel();
            this.txt_BlueTolerance = new System.Windows.Forms.NumericUpDown();
            this.lbl_Lightness = new SRMControl.SRMLabel();
            this.lbl_Saturation = new SRMControl.SRMLabel();
            this.txt_Green = new SRMControl.SRMInputBox();
            this.txt_Blue = new SRMControl.SRMInputBox();
            this.trackBar_Green = new System.Windows.Forms.TrackBar();
            this.pic_Green = new System.Windows.Forms.PictureBox();
            this.pic_Blue = new System.Windows.Forms.PictureBox();
            this.pic_Red = new System.Windows.Forms.PictureBox();
            this.trackBar_Blue = new System.Windows.Forms.TrackBar();
            this.lbl_Hue = new SRMControl.SRMLabel();
            this.trackBar_Red = new System.Windows.Forms.TrackBar();
            this.txt_Red = new SRMControl.SRMInputBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chk_Preview = new SRMControl.SRMCheckBox();
            this.pic_Threshold = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.txt_GreenTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_RedTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_BlueTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Green)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Green)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Blue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Red)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Blue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Red)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Threshold)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Cancel.Location = new System.Drawing.Point(208, 270);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 34);
            this.btn_Cancel.TabIndex = 81;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_OK.Location = new System.Drawing.Point(122, 270);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(80, 34);
            this.btn_OK.TabIndex = 80;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // lbl_HueTolerance
            // 
            this.lbl_HueTolerance.AutoSize = true;
            this.lbl_HueTolerance.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_HueTolerance.Location = new System.Drawing.Point(171, 23);
            this.lbl_HueTolerance.Name = "lbl_HueTolerance";
            this.lbl_HueTolerance.Size = new System.Drawing.Size(55, 13);
            this.lbl_HueTolerance.TabIndex = 104;
            this.lbl_HueTolerance.Text = "Tolerance";
            this.lbl_HueTolerance.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_SaturationTolerance
            // 
            this.lbl_SaturationTolerance.AutoSize = true;
            this.lbl_SaturationTolerance.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_SaturationTolerance.Location = new System.Drawing.Point(171, 106);
            this.lbl_SaturationTolerance.Name = "lbl_SaturationTolerance";
            this.lbl_SaturationTolerance.Size = new System.Drawing.Size(55, 13);
            this.lbl_SaturationTolerance.TabIndex = 102;
            this.lbl_SaturationTolerance.Text = "Tolerance";
            this.lbl_SaturationTolerance.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_GreenTolerance
            // 
            this.txt_GreenTolerance.Location = new System.Drawing.Point(228, 103);
            this.txt_GreenTolerance.Margin = new System.Windows.Forms.Padding(4);
            this.txt_GreenTolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_GreenTolerance.Name = "txt_GreenTolerance";
            this.txt_GreenTolerance.Size = new System.Drawing.Size(51, 20);
            this.txt_GreenTolerance.TabIndex = 101;
            this.txt_GreenTolerance.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_GreenTolerance.ValueChanged += new System.EventHandler(this.txt_GreenTolerance_ValueChanged);
            // 
            // txt_RedTolerance
            // 
            this.txt_RedTolerance.Location = new System.Drawing.Point(228, 20);
            this.txt_RedTolerance.Margin = new System.Windows.Forms.Padding(4);
            this.txt_RedTolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_RedTolerance.Name = "txt_RedTolerance";
            this.txt_RedTolerance.Size = new System.Drawing.Size(51, 20);
            this.txt_RedTolerance.TabIndex = 103;
            this.txt_RedTolerance.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_RedTolerance.ValueChanged += new System.EventHandler(this.txt_RedTolerance_ValueChanged);
            // 
            // lbl_LightnessTolerance
            // 
            this.lbl_LightnessTolerance.AutoSize = true;
            this.lbl_LightnessTolerance.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_LightnessTolerance.Location = new System.Drawing.Point(171, 187);
            this.lbl_LightnessTolerance.Name = "lbl_LightnessTolerance";
            this.lbl_LightnessTolerance.Size = new System.Drawing.Size(55, 13);
            this.lbl_LightnessTolerance.TabIndex = 100;
            this.lbl_LightnessTolerance.Text = "Tolerance";
            this.lbl_LightnessTolerance.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_BlueTolerance
            // 
            this.txt_BlueTolerance.Location = new System.Drawing.Point(228, 184);
            this.txt_BlueTolerance.Margin = new System.Windows.Forms.Padding(4);
            this.txt_BlueTolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_BlueTolerance.Name = "txt_BlueTolerance";
            this.txt_BlueTolerance.Size = new System.Drawing.Size(51, 20);
            this.txt_BlueTolerance.TabIndex = 99;
            this.txt_BlueTolerance.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_BlueTolerance.ValueChanged += new System.EventHandler(this.txt_BlueTolerance_ValueChanged);
            // 
            // lbl_Lightness
            // 
            this.lbl_Lightness.AutoSize = true;
            this.lbl_Lightness.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Lightness.Location = new System.Drawing.Point(19, 187);
            this.lbl_Lightness.Name = "lbl_Lightness";
            this.lbl_Lightness.Size = new System.Drawing.Size(28, 13);
            this.lbl_Lightness.TabIndex = 87;
            this.lbl_Lightness.Text = "Blue";
            this.lbl_Lightness.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // lbl_Saturation
            // 
            this.lbl_Saturation.AutoSize = true;
            this.lbl_Saturation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Saturation.Location = new System.Drawing.Point(19, 106);
            this.lbl_Saturation.Name = "lbl_Saturation";
            this.lbl_Saturation.Size = new System.Drawing.Size(36, 13);
            this.lbl_Saturation.TabIndex = 89;
            this.lbl_Saturation.Text = "Green";
            this.lbl_Saturation.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_Green
            // 
            this.txt_Green.BackColor = System.Drawing.Color.White;
            this.txt_Green.DecimalPlaces = 0;
            this.txt_Green.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Green.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Green.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Green.ForeColor = System.Drawing.Color.Black;
            this.txt_Green.InputType = SRMControl.InputType.Number;
            this.txt_Green.Location = new System.Drawing.Point(77, 102);
            this.txt_Green.Name = "txt_Green";
            this.txt_Green.NormalBackColor = System.Drawing.Color.White;
            this.txt_Green.Size = new System.Drawing.Size(55, 20);
            this.txt_Green.TabIndex = 90;
            this.txt_Green.Text = "125";
            this.txt_Green.TextChanged += new System.EventHandler(this.txt_Green_TextChanged);
            // 
            // txt_Blue
            // 
            this.txt_Blue.BackColor = System.Drawing.Color.White;
            this.txt_Blue.DecimalPlaces = 0;
            this.txt_Blue.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Blue.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Blue.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Blue.ForeColor = System.Drawing.Color.Black;
            this.txt_Blue.InputType = SRMControl.InputType.Number;
            this.txt_Blue.Location = new System.Drawing.Point(77, 184);
            this.txt_Blue.Name = "txt_Blue";
            this.txt_Blue.NormalBackColor = System.Drawing.Color.White;
            this.txt_Blue.Size = new System.Drawing.Size(55, 20);
            this.txt_Blue.TabIndex = 88;
            this.txt_Blue.Text = "125";
            this.txt_Blue.TextChanged += new System.EventHandler(this.txt_Blue_TextChanged);
            // 
            // trackBar_Green
            // 
            this.trackBar_Green.AutoSize = false;
            this.trackBar_Green.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Green.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.trackBar_Green.LargeChange = 1;
            this.trackBar_Green.Location = new System.Drawing.Point(6, 123);
            this.trackBar_Green.Maximum = 255;
            this.trackBar_Green.Name = "trackBar_Green";
            this.trackBar_Green.Size = new System.Drawing.Size(282, 23);
            this.trackBar_Green.TabIndex = 95;
            this.trackBar_Green.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Green.Value = 125;
            this.trackBar_Green.Scroll += new System.EventHandler(this.trackBar_Green_Scroll);
            // 
            // pic_Green
            // 
            this.pic_Green.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pic_Green.InitialImage = ((System.Drawing.Image)(resources.GetObject("pic_Green.InitialImage")));
            this.pic_Green.Location = new System.Drawing.Point(19, 149);
            this.pic_Green.Margin = new System.Windows.Forms.Padding(4);
            this.pic_Green.Name = "pic_Green";
            this.pic_Green.Size = new System.Drawing.Size(256, 15);
            this.pic_Green.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_Green.TabIndex = 96;
            this.pic_Green.TabStop = false;
            // 
            // pic_Blue
            // 
            this.pic_Blue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pic_Blue.InitialImage = ((System.Drawing.Image)(resources.GetObject("pic_Blue.InitialImage")));
            this.pic_Blue.Location = new System.Drawing.Point(19, 231);
            this.pic_Blue.Margin = new System.Windows.Forms.Padding(4);
            this.pic_Blue.Name = "pic_Blue";
            this.pic_Blue.Size = new System.Drawing.Size(256, 15);
            this.pic_Blue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_Blue.TabIndex = 98;
            this.pic_Blue.TabStop = false;
            // 
            // pic_Red
            // 
            this.pic_Red.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pic_Red.InitialImage = ((System.Drawing.Image)(resources.GetObject("pic_Red.InitialImage")));
            this.pic_Red.Location = new System.Drawing.Point(19, 67);
            this.pic_Red.Margin = new System.Windows.Forms.Padding(4);
            this.pic_Red.Name = "pic_Red";
            this.pic_Red.Size = new System.Drawing.Size(256, 15);
            this.pic_Red.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_Red.TabIndex = 94;
            this.pic_Red.TabStop = false;
            // 
            // trackBar_Blue
            // 
            this.trackBar_Blue.AutoSize = false;
            this.trackBar_Blue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Blue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.trackBar_Blue.LargeChange = 1;
            this.trackBar_Blue.Location = new System.Drawing.Point(6, 205);
            this.trackBar_Blue.Maximum = 255;
            this.trackBar_Blue.Name = "trackBar_Blue";
            this.trackBar_Blue.Size = new System.Drawing.Size(282, 23);
            this.trackBar_Blue.TabIndex = 97;
            this.trackBar_Blue.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Blue.Value = 125;
            this.trackBar_Blue.Scroll += new System.EventHandler(this.trackBar_Blue_Scroll);
            // 
            // lbl_Hue
            // 
            this.lbl_Hue.AutoSize = true;
            this.lbl_Hue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Hue.Location = new System.Drawing.Point(19, 23);
            this.lbl_Hue.Name = "lbl_Hue";
            this.lbl_Hue.Size = new System.Drawing.Size(27, 13);
            this.lbl_Hue.TabIndex = 92;
            this.lbl_Hue.Text = "Red";
            this.lbl_Hue.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // trackBar_Red
            // 
            this.trackBar_Red.AutoSize = false;
            this.trackBar_Red.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.trackBar_Red.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.trackBar_Red.LargeChange = 1;
            this.trackBar_Red.Location = new System.Drawing.Point(6, 41);
            this.trackBar_Red.Maximum = 255;
            this.trackBar_Red.Name = "trackBar_Red";
            this.trackBar_Red.Size = new System.Drawing.Size(282, 23);
            this.trackBar_Red.TabIndex = 91;
            this.trackBar_Red.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_Red.Value = 125;
            this.trackBar_Red.Scroll += new System.EventHandler(this.trackBar_Red_Scroll);
            // 
            // txt_Red
            // 
            this.txt_Red.BackColor = System.Drawing.Color.White;
            this.txt_Red.DecimalPlaces = 0;
            this.txt_Red.DecMaxValue = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txt_Red.DecMinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Red.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txt_Red.ForeColor = System.Drawing.Color.Black;
            this.txt_Red.InputType = SRMControl.InputType.Number;
            this.txt_Red.Location = new System.Drawing.Point(77, 20);
            this.txt_Red.Name = "txt_Red";
            this.txt_Red.NormalBackColor = System.Drawing.Color.White;
            this.txt_Red.Size = new System.Drawing.Size(55, 20);
            this.txt_Red.TabIndex = 93;
            this.txt_Red.Text = "125";
            this.txt_Red.TextChanged += new System.EventHandler(this.txt_Red_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // chk_Preview
            // 
            this.chk_Preview.Checked = true;
            this.chk_Preview.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_Preview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Preview.Location = new System.Drawing.Point(13, 270);
            this.chk_Preview.Name = "chk_Preview";
            this.chk_Preview.Selected = false;
            this.chk_Preview.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_Preview.Size = new System.Drawing.Size(103, 24);
            this.chk_Preview.TabIndex = 105;
            this.chk_Preview.Text = "Preview";
            this.chk_Preview.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_Preview.UseVisualStyleBackColor = true;
            this.chk_Preview.Click += new System.EventHandler(this.chk_Preview_Click);
            // 
            // pic_Threshold
            // 
            this.pic_Threshold.Location = new System.Drawing.Point(-2, 310);
            this.pic_Threshold.Name = "pic_Threshold";
            this.pic_Threshold.Size = new System.Drawing.Size(296, 222);
            this.pic_Threshold.TabIndex = 106;
            this.pic_Threshold.TabStop = false;
            this.pic_Threshold.Paint += new System.Windows.Forms.PaintEventHandler(this.pic_Threshold_Paint);
            // 
            // RGBThresholdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(292, 551);
            this.ControlBox = false;
            this.Controls.Add(this.pic_Threshold);
            this.Controls.Add(this.chk_Preview);
            this.Controls.Add(this.lbl_HueTolerance);
            this.Controls.Add(this.lbl_SaturationTolerance);
            this.Controls.Add(this.txt_GreenTolerance);
            this.Controls.Add(this.txt_RedTolerance);
            this.Controls.Add(this.lbl_LightnessTolerance);
            this.Controls.Add(this.txt_BlueTolerance);
            this.Controls.Add(this.lbl_Lightness);
            this.Controls.Add(this.lbl_Saturation);
            this.Controls.Add(this.txt_Green);
            this.Controls.Add(this.txt_Blue);
            this.Controls.Add(this.trackBar_Green);
            this.Controls.Add(this.pic_Green);
            this.Controls.Add(this.pic_Blue);
            this.Controls.Add(this.pic_Red);
            this.Controls.Add(this.trackBar_Blue);
            this.Controls.Add(this.lbl_Hue);
            this.Controls.Add(this.trackBar_Red);
            this.Controls.Add(this.txt_Red);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RGBThresholdForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Color Threshold";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RGBThresholdForm_FormClosing);
            this.Load += new System.EventHandler(this.RGBThresholdForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txt_GreenTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_RedTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_BlueTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Green)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Green)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Blue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Red)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Blue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Red)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Threshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private SRMControl.SRMLabel lbl_HueTolerance;
        private SRMControl.SRMLabel lbl_SaturationTolerance;
        private System.Windows.Forms.NumericUpDown txt_GreenTolerance;
        private System.Windows.Forms.NumericUpDown txt_RedTolerance;
        private SRMControl.SRMLabel lbl_LightnessTolerance;
        private System.Windows.Forms.NumericUpDown txt_BlueTolerance;
        private SRMControl.SRMLabel lbl_Lightness;
        private SRMControl.SRMLabel lbl_Saturation;
        private SRMControl.SRMInputBox txt_Green;
        private SRMControl.SRMInputBox txt_Blue;
        private System.Windows.Forms.TrackBar trackBar_Green;
        private System.Windows.Forms.PictureBox pic_Green;
        private System.Windows.Forms.PictureBox pic_Blue;
        private System.Windows.Forms.PictureBox pic_Red;
        private System.Windows.Forms.TrackBar trackBar_Blue;
        private SRMControl.SRMLabel lbl_Hue;
        private System.Windows.Forms.TrackBar trackBar_Red;
        private SRMControl.SRMInputBox txt_Red;
        private System.Windows.Forms.Timer timer1;
        private SRMControl.SRMCheckBox chk_Preview;
        private System.Windows.Forms.PictureBox pic_Threshold;
    }
}