namespace VisionProcessForm
{
    partial class PosCrosshairSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PosCrosshairSettingForm));
            this.btn_Cancel = new SRMControl.SRMButton();
            this.btn_OK = new SRMControl.SRMButton();
            this.txt_CenterY = new System.Windows.Forms.NumericUpDown();
            this.txt_CenterX = new System.Windows.Forms.NumericUpDown();
            this.srmLabel1 = new SRMControl.SRMLabel();
            this.srmLabel13 = new SRMControl.SRMLabel();
            this.txt_ROIHeight = new System.Windows.Forms.NumericUpDown();
            this.txt_ROIWidth = new System.Windows.Forms.NumericUpDown();
            this.srmLabel2 = new SRMControl.SRMLabel();
            this.srmLabel3 = new SRMControl.SRMLabel();
            this.btn_Close = new SRMControl.SRMButton();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_ROIHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_ROIWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // txt_CenterY
            // 
            resources.ApplyResources(this.txt_CenterY, "txt_CenterY");
            this.txt_CenterY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_CenterY.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txt_CenterY.Name = "txt_CenterY";
            this.txt_CenterY.ValueChanged += new System.EventHandler(this.txt_Center_ValueChanged);
            // 
            // txt_CenterX
            // 
            resources.ApplyResources(this.txt_CenterX, "txt_CenterX");
            this.txt_CenterX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txt_CenterX.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txt_CenterX.Name = "txt_CenterX";
            this.txt_CenterX.ValueChanged += new System.EventHandler(this.txt_Center_ValueChanged);
            // 
            // srmLabel1
            // 
            resources.ApplyResources(this.srmLabel1, "srmLabel1");
            this.srmLabel1.Name = "srmLabel1";
            this.srmLabel1.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel13
            // 
            resources.ApplyResources(this.srmLabel13, "srmLabel13");
            this.srmLabel13.Name = "srmLabel13";
            this.srmLabel13.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // txt_ROIHeight
            // 
            resources.ApplyResources(this.txt_ROIHeight, "txt_ROIHeight");
            this.txt_ROIHeight.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_ROIHeight.Name = "txt_ROIHeight";
            this.txt_ROIHeight.ValueChanged += new System.EventHandler(this.txt_ROISize_ValueChanged);
            // 
            // txt_ROIWidth
            // 
            resources.ApplyResources(this.txt_ROIWidth, "txt_ROIWidth");
            this.txt_ROIWidth.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.txt_ROIWidth.Name = "txt_ROIWidth";
            this.txt_ROIWidth.ValueChanged += new System.EventHandler(this.txt_ROISize_ValueChanged);
            // 
            // srmLabel2
            // 
            resources.ApplyResources(this.srmLabel2, "srmLabel2");
            this.srmLabel2.Name = "srmLabel2";
            this.srmLabel2.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // srmLabel3
            // 
            resources.ApplyResources(this.srmLabel3, "srmLabel3");
            this.srmLabel3.Name = "srmLabel3";
            this.srmLabel3.TextShadowColor = System.Drawing.Color.Gray;
            // 
            // btn_Close
            // 
            resources.ApplyResources(this.btn_Close, "btn_Close");
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // PosCrosshairSettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ControlBox = false;
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.txt_ROIHeight);
            this.Controls.Add(this.txt_ROIWidth);
            this.Controls.Add(this.srmLabel2);
            this.Controls.Add(this.srmLabel3);
            this.Controls.Add(this.txt_CenterY);
            this.Controls.Add(this.txt_CenterX);
            this.Controls.Add(this.srmLabel1);
            this.Controls.Add(this.srmLabel13);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PosCrosshairSettingForm";
            this.Load += new System.EventHandler(this.PosCrosshairSettingForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PosCrosshairSettingForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CenterX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_ROIHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_ROIWidth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SRMControl.SRMButton btn_Cancel;
        private SRMControl.SRMButton btn_OK;
        private System.Windows.Forms.NumericUpDown txt_CenterY;
        private System.Windows.Forms.NumericUpDown txt_CenterX;
        private SRMControl.SRMLabel srmLabel1;
        private SRMControl.SRMLabel srmLabel13;
        private System.Windows.Forms.NumericUpDown txt_ROIHeight;
        private System.Windows.Forms.NumericUpDown txt_ROIWidth;
        private SRMControl.SRMLabel srmLabel2;
        private SRMControl.SRMLabel srmLabel3;
        private SRMControl.SRMButton btn_Close;
    }
}