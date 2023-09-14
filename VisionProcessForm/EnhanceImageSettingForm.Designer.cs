namespace VisionProcessForm
{
    partial class EnchanceImageSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnchanceImageSettingForm));
            this.txt_Gain = new System.Windows.Forms.NumericUpDown();
            this.txt_Offset = new System.Windows.Forms.NumericUpDown();
            this.txt_Dilate = new System.Windows.Forms.NumericUpDown();
            this.txt_Open = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_Save = new SRMControl.SRMButton();
            this.pnl_ExtraGain = new System.Windows.Forms.Panel();
            this.pnl_ExtraGain4 = new System.Windows.Forms.Panel();
            this.pnl_ExtraGain3 = new System.Windows.Forms.Panel();
            this.pnl_ExtraGain2 = new System.Windows.Forms.Panel();
            this.pnl_ExtraGain1 = new System.Windows.Forms.Panel();
            this.chk_EnableContrast = new SRMControl.SRMCheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_Close = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Gain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Offset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Dilate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Open)).BeginInit();
            this.pnl_ExtraGain.SuspendLayout();
            this.pnl_ExtraGain4.SuspendLayout();
            this.pnl_ExtraGain3.SuspendLayout();
            this.pnl_ExtraGain2.SuspendLayout();
            this.pnl_ExtraGain1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Close)).BeginInit();
            this.SuspendLayout();
            // 
            // txt_Gain
            // 
            resources.ApplyResources(this.txt_Gain, "txt_Gain");
            this.txt_Gain.DecimalPlaces = 1;
            this.txt_Gain.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.txt_Gain.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_Gain.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Gain.Name = "txt_Gain";
            this.txt_Gain.Tag = "3";
            this.txt_Gain.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Gain.ValueChanged += new System.EventHandler(this.txt_Gain_ValueChanged);
            // 
            // txt_Offset
            // 
            resources.ApplyResources(this.txt_Offset, "txt_Offset");
            this.txt_Offset.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txt_Offset.Minimum = new decimal(new int[] {
            255,
            0,
            0,
            -2147483648});
            this.txt_Offset.Name = "txt_Offset";
            this.txt_Offset.Tag = "2";
            this.txt_Offset.Value = new decimal(new int[] {
            50,
            0,
            0,
            -2147483648});
            this.txt_Offset.ValueChanged += new System.EventHandler(this.txt_Offset_ValueChanged);
            // 
            // txt_Dilate
            // 
            resources.ApplyResources(this.txt_Dilate, "txt_Dilate");
            this.txt_Dilate.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_Dilate.Name = "txt_Dilate";
            this.txt_Dilate.Tag = "1";
            this.txt_Dilate.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Dilate.ValueChanged += new System.EventHandler(this.txt_Dilate_ValueChanged);
            // 
            // txt_Open
            // 
            resources.ApplyResources(this.txt_Open, "txt_Open");
            this.txt_Open.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_Open.Name = "txt_Open";
            this.txt_Open.Tag = "0";
            this.txt_Open.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Open.ValueChanged += new System.EventHandler(this.txt_Open_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // pnl_ExtraGain
            // 
            resources.ApplyResources(this.pnl_ExtraGain, "pnl_ExtraGain");
            this.pnl_ExtraGain.Controls.Add(this.pnl_ExtraGain4);
            this.pnl_ExtraGain.Controls.Add(this.pnl_ExtraGain3);
            this.pnl_ExtraGain.Controls.Add(this.pnl_ExtraGain2);
            this.pnl_ExtraGain.Controls.Add(this.pnl_ExtraGain1);
            this.pnl_ExtraGain.Name = "pnl_ExtraGain";
            // 
            // pnl_ExtraGain4
            // 
            resources.ApplyResources(this.pnl_ExtraGain4, "pnl_ExtraGain4");
            this.pnl_ExtraGain4.Controls.Add(this.label4);
            this.pnl_ExtraGain4.Controls.Add(this.txt_Gain);
            this.pnl_ExtraGain4.Name = "pnl_ExtraGain4";
            // 
            // pnl_ExtraGain3
            // 
            resources.ApplyResources(this.pnl_ExtraGain3, "pnl_ExtraGain3");
            this.pnl_ExtraGain3.Controls.Add(this.label2);
            this.pnl_ExtraGain3.Controls.Add(this.txt_Offset);
            this.pnl_ExtraGain3.Name = "pnl_ExtraGain3";
            // 
            // pnl_ExtraGain2
            // 
            resources.ApplyResources(this.pnl_ExtraGain2, "pnl_ExtraGain2");
            this.pnl_ExtraGain2.Controls.Add(this.label1);
            this.pnl_ExtraGain2.Controls.Add(this.txt_Dilate);
            this.pnl_ExtraGain2.Name = "pnl_ExtraGain2";
            // 
            // pnl_ExtraGain1
            // 
            resources.ApplyResources(this.pnl_ExtraGain1, "pnl_ExtraGain1");
            this.pnl_ExtraGain1.Controls.Add(this.label3);
            this.pnl_ExtraGain1.Controls.Add(this.txt_Open);
            this.pnl_ExtraGain1.Name = "pnl_ExtraGain1";
            // 
            // chk_EnableContrast
            // 
            resources.ApplyResources(this.chk_EnableContrast, "chk_EnableContrast");
            this.chk_EnableContrast.CheckedColor = System.Drawing.Color.GreenYellow;
            this.chk_EnableContrast.Name = "chk_EnableContrast";
            this.chk_EnableContrast.Selected = true;
            this.chk_EnableContrast.SelectedBorderColor = System.Drawing.Color.Red;
            this.chk_EnableContrast.UnCheckedColor = System.Drawing.Color.Red;
            this.chk_EnableContrast.UseVisualStyleBackColor = true;
            this.chk_EnableContrast.Click += new System.EventHandler(this.chk_EnableContrast_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txt_Close);
            this.panel1.Name = "panel1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // txt_Close
            // 
            resources.ApplyResources(this.txt_Close, "txt_Close");
            this.txt_Close.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txt_Close.Name = "txt_Close";
            this.txt_Close.Tag = "0";
            this.txt_Close.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_Close.ValueChanged += new System.EventHandler(this.txt_Close_ValueChanged);
            // 
            // EnchanceImageSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chk_EnableContrast);
            this.Controls.Add(this.pnl_ExtraGain);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnchanceImageSettingForm";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.txt_Gain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Offset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Dilate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Open)).EndInit();
            this.pnl_ExtraGain.ResumeLayout(false);
            this.pnl_ExtraGain4.ResumeLayout(false);
            this.pnl_ExtraGain4.PerformLayout();
            this.pnl_ExtraGain3.ResumeLayout(false);
            this.pnl_ExtraGain3.PerformLayout();
            this.pnl_ExtraGain2.ResumeLayout(false);
            this.pnl_ExtraGain2.PerformLayout();
            this.pnl_ExtraGain1.ResumeLayout(false);
            this.pnl_ExtraGain1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Close)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.NumericUpDown txt_Gain;
        private System.Windows.Forms.NumericUpDown txt_Offset;
        private System.Windows.Forms.NumericUpDown txt_Dilate;
        private System.Windows.Forms.NumericUpDown txt_Open;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_Save;
        private System.Windows.Forms.Panel pnl_ExtraGain;
        private System.Windows.Forms.Panel pnl_ExtraGain4;
        private System.Windows.Forms.Panel pnl_ExtraGain3;
        private System.Windows.Forms.Panel pnl_ExtraGain2;
        private System.Windows.Forms.Panel pnl_ExtraGain1;
        private SRMControl.SRMCheckBox chk_EnableContrast;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown txt_Close;
    }
}